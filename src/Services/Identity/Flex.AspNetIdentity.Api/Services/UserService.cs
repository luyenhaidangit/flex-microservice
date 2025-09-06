using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Integrations.Interfaces;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Flex.AspNetIdentity.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IdentityDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserRequestRepository _userRequestRepository;
        private readonly ICurrentUserService _userService;
        private readonly IBranchIntegrationService _branchIntegrationService;

        public UserService(
            ILogger<UserService> logger,
            IdentityDbContext dbContext,
            ICurrentUserService userService,
            IUserRepository userRepository,
            IUserRequestRepository userRequestRepository,
            IBranchIntegrationService branchIntegrationService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userService = userService;
            _userRepository = userRepository;
            _userRequestRepository = userRequestRepository;
            _branchIntegrationService = branchIntegrationService;
        }

        #region Query

        /// <summary>
        /// Get all approved users with pagination.
        /// </summary>
        public async Task<PagedResult<UserPagingDto>> GetUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct)
        {
            // ===== Process request parameters =====
            var keyword = request.Keyword?.Trim().ToLower();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            // ===== Build query =====
            var query = _userRepository.FindAll().AsNoTracking()
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
                .WhereIf(request.BranchId.HasValue, u => u.BranchId == request.BranchId!.Value);

            // ===== Execute query =====
            var total = await query.CountAsync(ct);
            var raw = await query
                .OrderBy(u => u.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new { u.UserName, u.FullName, u.Email, u.PhoneNumber, u.BranchId, u.LockoutEnd, u.IsActive })
                .ToListAsync(ct);

            // ===== Get branch information =====
            var branchIds = raw.Where(u => u.BranchId > 0).Select(u => u.BranchId).Distinct().ToList();
            var branches = new Dictionary<long, string>();
            if (branchIds.Any())
            {
                var branchDtos = await _branchIntegrationService.BatchGetBranchesAsync(branchIds, ct);
                branches = branchDtos.ToDictionary(b => b.Id, b => b.Name);
            }

            // ===== Build result =====
            var items = raw.Select(u => new UserPagingDto
            {
                UserName = u.UserName ?? string.Empty,
                FullName = u.FullName,
                Email = u.Email ?? "",
                BranchName = branches.TryGetValue(u.BranchId, out var branchName) ? branchName : "",
                IsActive = u.IsActive,
            }).ToList();

            // ===== Return result =====
            return PagedResult<UserPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get all pending user requests with pagination.
        /// </summary>
        public async Task<PagedResult<UserPendingPagingDto>> GetPendingUserRequestsPagedAsync(GetUserRequestsPagingRequest request, CancellationToken ct)
        {
            // ===== Process request parameters =====
            var keyword = request.Keyword?.Trim().ToLower();
            var requestType = request.Type?.Trim().ToUpperInvariant();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            // ===== Build query using view =====
            var pendingQuery = _userRequestRepository.GetAllUserRequests()
                .WhereIf(!string.IsNullOrEmpty(keyword), 
                    r => EF.Functions.Like(r.UserName.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.FullName.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.Email.ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestTypeConstant.All, 
                    r => r.Action == requestType)
                .AsNoTracking()
                .Select(r => new UserPendingPagingDto
                {
                    RequestId = r.RequestId,
                    Action = r.Action,
                    RequestedBy = r.RequestedBy,
                    RequestedDate = r.RequestedDate,
                    UserName = r.UserName,
                    FullName = r.FullName,
                    Email = r.Email
                });

            // ===== Execute query =====
            var total = await pendingQuery.CountAsync(ct);
            var items = await pendingQuery
                .OrderByDescending(dto => dto.RequestedDate)
                .ThenBy(dto => dto.RequestId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // ===== Return result =====
            return PagedResult<UserPendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get approved user by username.
        /// </summary>
        public async Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken ct)
        {
            // ===== Find user by username =====
            var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower(), ct);

            if(user == null)
            {
                throw new Exception($"User '{userName}' not found.");
            };

            // ===== Get branch information =====
            string branchName = "";
            if (user.BranchId > 0)
            {
                try
                {
                    var branch = await _branchIntegrationService.GetBranchByIdAsync(user.BranchId, ct);
                    branchName = branch?.Name ?? "";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to retrieve branch information for user {UserName} with BranchId {BranchId}", userName, user.BranchId);
                }
            }

            // ===== Get user roles =====
            var roles = await _dbContext.Set<UserRole>()
                .Where(ur => ur.UserId == user.Id)
                .Join(_dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r.Code)
                .ToListAsync(ct);

            // ===== Return result =====
            return new UserDetailDto
            {
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email,
                BranchName = branchName,
                IsActive = user.IsActive,
                Roles = roles
            };
        }

        /// <summary>
        /// Get user change history by username.
        /// </summary>
        public async Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName)
        {
            // ===== Find user with username =====
            var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower());

            if (user == null)
            {
                throw new Exception($"User with username '{userName}' not exists.");
            }

            // ===== Get user histories by user Id =====
            var userId = user.Id;

            var requests = await _userRequestRepository.FindAll()
                .Where(r => r.EntityId == userId).AsNoTracking()
                .OrderByDescending(r => r.RequestedDate)
                .Select(r => new
                {
                    r.Id,
                    r.MakerId,
                    r.RequestedDate,
                    r.CheckerId,
                    r.ApproveDate,
                    r.Status,
                    r.Comments,
                    r.RequestedData
                })
                .AsNoTracking()
                .ToListAsync();

            // ===== Build result =====
            var historyItems = requests.Select((req, idx) => new UserChangeHistoryDto
            {
                Id = req.Id,
                MakerBy = req.MakerId,
                MakerTime = req.RequestedDate,
                ApproverBy = req.CheckerId,
                ApproverTime = req.ApproveDate,
                Status = req.Status,
                Description = req.Comments,
                Changes = req.RequestedData
            }).ToList();

            return historyItems;
        }
        #endregion

        #region Create/Update/Delete immediate with audit comment

        /// <summary>
        /// Create user immediately.
        /// </summary>
        public async Task<long> CreateUserRequestAsync(CreateUserRequest request)
        {
            // ===== Validate request =====
            // Check if user already exists
            var existingUser = await _userRepository.FindAll().AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName!.ToLower() == request.UserName.ToLower());
            
            if (existingUser != null)
            {
                throw new Exception($"User with username '{request.UserName}' already exists.");
            }

            // Check if user request already exists with pending status
            var existingPendingRequest = await _userRequestRepository.GetAllUserRequests()
                .Where(ur => ur.UserName.ToLower() == request.UserName.ToLower() && ur.Status == RequestStatusConstant.Unauthorised)
                .FirstOrDefaultAsync();
            
            if (existingPendingRequest != null)
            {
                throw new Exception($"A pending request with username '{request.UserName}' already exists.");
            }

            // ===== Process =====
            var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
            var requestedJson = JsonSerializer.Serialize(request);
            var requestDto = new UserRequest
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = 0,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                Comments = "Yêu cầu thêm mới người sử dụng.",
                RequestedData = requestedJson
            };
            await _userRequestRepository.CreateAsync(requestDto);

            return requestDto.Id;
        }

        /// <summary>
        /// Create update user request.
        /// </summary>
        public async Task<long> UpdateUserRequestAsync(UpdateUserRequest request)
        {
            // ===== Validation =====
            // ===== Check user exists =====
            var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                throw new Exception($"User with username '{request.UserName}' does not exist.");
            }

            // ===== Check user request exists =====
            if (user.Status == RequestStatusConstant.Unauthorised)
            {
                throw new Exception("A pending update request already exists for this user.");
            }

            // ===== Process =====
            // ===== Create update user request =====
            var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
            var requestedJson = JsonSerializer.Serialize(request);
            var userRequest = new UserRequest
            {
                Action = RequestTypeConstant.Update,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = user.Id,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson,
                Comments = request.Comment ?? "Yêu cầu cập nhật người dùng."
            };

            // ===== Update status process user =====
            user.Status = RequestStatusConstant.Unauthorised;

            // ===== Transaction =====
            await using var transaction = await _userRequestRepository.BeginTransactionAsync();
            try
            {
                await _userRequestRepository.CreateAsync(userRequest);
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to create update user request.");
            }

            return userRequest.Id;
        }

        /// <summary>
        /// Delete user immediately.
        /// </summary>
        public async Task<long> DeleteUserRequestAsync(string userName)
        {
            // ===== Find and soft-delete user =====
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _dbContext.SaveChangesAsync();

            return user.Id;
        }

        /// <summary>
        /// Get pending user request detail by request ID.
        /// </summary>
        public async Task<UserRequestDetailDto> GetPendingUserRequestByIdAsync(long requestId)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new ArgumentException($"User request with ID {requestId} not found.");
            }

            // ===== Build base result =====
            var result = new UserRequestDetailDto
            {
                RequestId = request.Id.ToString(),
                Type = request.Action,
            };

            // ===== Process request data based on action type =====
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    ProcessCreateUserRequestData(request, result);
                    break;
                case RequestTypeConstant.Update:
                    await ProcessUpdateUserRequestData(request, result);
                    break;
                case RequestTypeConstant.Delete:
                    await ProcessDeleteUserRequestData(request, result);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Approve pending user request by ID.
        /// </summary>
        public async Task<UserRequestApprovalResultDto> ApprovePendingUserRequestAsync(long requestId, string? comment = null)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new ArgumentException($"User request with ID {requestId} not found.");
            }

            if (request.Status != RequestStatusConstant.Unauthorised)
            {
                throw new ArgumentException($"User request with ID {requestId} is not in pending status.");
            }

            // ===== Process approval based on action type =====
            long? createdUserId = null;
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    createdUserId = await ProcessCreateUserApproval(request);
                    break;
                case RequestTypeConstant.Update:
                    await ProcessUpdateUserApproval(request);
                    break;
                case RequestTypeConstant.Delete:
                    await ProcessDeleteUserApproval(request);
                    break;
            }

            // ===== Return result =====
            return new UserRequestApprovalResultDto
            {
                RequestId = requestId,
                RequestType = request.Action,
                Status = RequestStatusConstant.Authorised,
                Comment = comment,
                CreatedUserId = createdUserId
            };
        }

        /// <summary>
        /// Reject pending user request by ID.
        /// </summary>
        public async Task<UserRequestApprovalResultDto> RejectPendingUserRequestAsync(long requestId, string? reason = null)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new ArgumentException($"User request with ID {requestId} not found.");
            }

            if (request.Status != RequestStatusConstant.Unauthorised)
            {
                throw new ArgumentException($"User request with ID {requestId} is not in pending status.");
            }

            // ===== Return result =====
            return new UserRequestApprovalResultDto
            {
                RequestId = requestId,
                RequestType = request.Action,
                Status = RequestStatusConstant.Rejected,
                Comment = reason
            };
        }

        #region Private Helper Methods for User Requests

        private static string ExtractUserNameFromRequestData(string requestData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(requestData);
                return data?.GetValueOrDefault("UserName")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ExtractFullNameFromRequestData(string requestData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(requestData);
                return data?.GetValueOrDefault("FullName")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ExtractEmailFromRequestData(string requestData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(requestData);
                return data?.GetValueOrDefault("Email")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ExtractPhoneNumberFromRequestData(string requestData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(requestData);
                return data?.GetValueOrDefault("PhoneNumber")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void ProcessCreateUserRequestData(UserRequest request, UserRequestDetailDto result)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData);
                result.NewData = new UserDetailDataDto
                {
                    UserName = data?.GetValueOrDefault("UserName")?.ToString() ?? string.Empty,
                    FullName = data?.GetValueOrDefault("FullName")?.ToString(),
                    Email = data?.GetValueOrDefault("Email")?.ToString(),
                    PhoneNumber = data?.GetValueOrDefault("PhoneNumber")?.ToString(),
                    BranchId = data?.GetValueOrDefault("BranchId")?.ToString() != null ? long.Parse(data["BranchId"].ToString()!) : null
                };
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
        }

        private async Task ProcessUpdateUserRequestData(UserRequest request, UserRequestDetailDto result)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData);
                var userName = data?.GetValueOrDefault("UserName")?.ToString();
                
                if (!string.IsNullOrEmpty(userName))
                {
                    // ===== Get current user data =====
                    var currentUser = await _userRepository.FindAll()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UserName == userName);

                    if (currentUser != null)
                    {
                        result.OldData = new UserDetailDataDto
                        {
                            UserName = currentUser.UserName ?? string.Empty,
                            FullName = currentUser.FullName,
                            Email = currentUser.Email,
                            PhoneNumber = currentUser.PhoneNumber,
                            BranchId = currentUser.BranchId
                        };
                    }
                }

                result.NewData = new UserDetailDataDto
                {
                    UserName = data?.GetValueOrDefault("UserName")?.ToString() ?? string.Empty,
                    FullName = data?.GetValueOrDefault("FullName")?.ToString(),
                    Email = data?.GetValueOrDefault("Email")?.ToString(),
                    PhoneNumber = data?.GetValueOrDefault("PhoneNumber")?.ToString(),
                    BranchId = data?.GetValueOrDefault("BranchId")?.ToString() != null ? long.Parse(data["BranchId"].ToString()!) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process update user request data for request {RequestId}", request.Id);
            }
        }

        private async Task ProcessDeleteUserRequestData(UserRequest request, UserRequestDetailDto result)
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData);
                var userName = data?.GetValueOrDefault("UserName")?.ToString();
                
                if (!string.IsNullOrEmpty(userName))
                {
                    // ===== Get current user data =====
                    var currentUser = await _userRepository.FindAll()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UserName == userName);

                    if (currentUser != null)
                    {
                        result.OldData = new UserDetailDataDto
                        {
                            UserName = currentUser.UserName ?? string.Empty,
                            FullName = currentUser.FullName,
                            Email = currentUser.Email,
                            PhoneNumber = currentUser.PhoneNumber,
                            BranchId = currentUser.BranchId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process delete user request data for request {RequestId}", request.Id);
            }
        }

        private async Task<long> ProcessCreateUserApproval(UserRequest request)
        {
            try
            {
                // ===== Deserialize and validate data =====
                var data = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
                if (data == null)
                {
                    throw new ArgumentException("Invalid request data for user creation.");
                }

                // ===== Create user =====
                var userId = await this.CreateUserRequestAsync(data);
                
                // ===== Parse userId to long =====
                //if (long.TryParse(userId, out var userIdLong))
                //{
                //    return userIdLong;
                //}
                
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process create user approval for request {RequestId}", request.Id);
                throw;
            }
        }

        private async Task ProcessUpdateUserApproval(UserRequest request)
        {
            try
            {
                // ===== Deserialize and validate data =====
                var data = JsonSerializer.Deserialize<UpdateUserRequest>(request.RequestedData);
                if (data == null)
                {
                    throw new ArgumentException("Invalid request data for user update.");
                }

                var userName = ExtractUserNameFromRequestData(request.RequestedData);
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("UserName is required for user update.");
                }

                // ===== Update user =====
                //await UpdateUserAsync(userName, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process update user approval for request {RequestId}", request.Id);
                throw;
            }
        }

        private async Task ProcessDeleteUserApproval(UserRequest request)
        {
            try
            {
                // ===== Deserialize and validate data =====
                var data = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
                if (data == null)
                {
                    throw new ArgumentException("Invalid request data for user deletion.");
                }

                var userName = ExtractUserNameFromRequestData(request.RequestedData);
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("UserName is required for user deletion.");
                }

                // ===== Delete user =====
                //await DeleteUserAsync(userName, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process delete user approval for request {RequestId}", request.Id);
                throw;
            }
        }

        #endregion
        #endregion
    }
}