using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Integrations.Interfaces;
using Flex.AspNetIdentity.Api.Models.Branch;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Infrastructure.Exceptions;
using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            ILogger<UserService> logger,
            IdentityDbContext dbContext,
            ICurrentUserService userService,
            IUserRepository userRepository,
            IUserRequestRepository userRequestRepository,
            IBranchIntegrationService branchIntegrationService,
            IPasswordHasher<User> passwordHasher)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userService = userService;
            _userRepository = userRepository;
            _userRequestRepository = userRequestRepository;
            _branchIntegrationService = branchIntegrationService;
            _passwordHasher = passwordHasher;
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
        /// Get approved user by username.
        /// </summary>
        public async Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken ct)
        {
            // ===== Find user by username =====
            var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower(), ct);

            if (user == null)
            {
                throw new Exception($"User '{userName}' not found.");
            }
            ;

            // ===== Get branch information =====
            var branch = new BranchLookupDto(0, "");
            if (user.BranchId > 0)
            {
                try
                {
                    branch = await _branchIntegrationService.GetBranchByIdAsync(user.BranchId, ct) ?? new BranchLookupDto(0, "");
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
                BranchName = branch.Name,
                IsActive = user.IsActive,
                Roles = roles,
                Branch = branch ?? new BranchLookupDto(0, "")
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

            var requests = await _userRequestRepository.FindByCondition(r => r.EntityId == userId)
                .AsNoTracking()
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
                .Where(x => x.Status == RequestStatusConstant.Unauthorised)
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
        /// Get pending user request detail by request ID.
        /// </summary>
        public async Task<PendingRequestDtoBase<UserRequestDataDto>> GetPendingUserRequestByIdAsync(long requestId)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (request == null)
            {
                throw new ValidationException(ErrorCode.RequestNotFound);
            }

            // ===== Build base result =====
            var result = new PendingRequestDtoBase<UserRequestDataDto>
            {
                RequestId = request.Id.ToString(),
                Type = request.Action,
                CreatedBy = request.MakerId.ToString(),
                CreatedDate = request.RequestedDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // ===== Process request data based on action type =====
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    ConvertCreateUserRequestData(request, result);
                    break;
                case RequestTypeConstant.Update:
                    await ProcessUpdateUserRequestData(request, result);
                    break;
                case RequestTypeConstant.Delete:
                    await ProcessDeleteUserRequestData(request, result);
                    break;
                default:
                    throw new ValidationException(ErrorCode.InvalidRequestType);
            }

            return result;
        }
        #endregion

        #region Command

        /// <summary>
        /// Create user immediately.
        /// </summary>
        public async Task<long> CreateUserRequestAsync(CreateUserRequest request)
        {
            // ===== Validate =====
            var isValid = await ValidateCreateUserRequestAsync(request);
            if (!isValid)
            {
                throw new ValidationException(ErrorCode.InvalidRequest);
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
            var email = request.Email?.ToLower();

            // ===== Validation =====
            // ===== Check user exists =====
            var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                throw new Exception($"User with username '{request.UserName}' does not exist.");
            }

            // ===== Check email uniqueness (if email is being changed) =====
            if (!string.IsNullOrEmpty(email) && user.Email?.ToLower() != email)
            {
                if (await _userRepository.ExistsByEmailAsync(email))
                {
                    throw new ValidationException(ErrorCode.EmailAlreadyExists);
                }
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
                //Comments = request.Comment ?? "Yêu cầu cập nhật người dùng."
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
        /// Approve pending user request by ID.
        /// </summary>
        public async Task<bool> ApprovePendingUserRequestAsync(long requestId)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                throw new ValidationException(ErrorCode.RequestNotFound);
            }

            if (request.Status != RequestStatusConstant.Unauthorised)
            {
                throw new ValidationException(ErrorCode.RequestNotPending);
            }

            // ===== Process approval with transaction =====
            var approver = _userService.GetCurrentUsername() ?? "system";

            await using var transaction = await _userRequestRepository.BeginTransactionAsync();
            try
            {
                // Process approval based on action type
                switch (request.Action)
                {
                    case RequestTypeConstant.Create:
                        await ProcessCreateUserApproval(request);
                        break;
                    case RequestTypeConstant.Update:
                        await ProcessUpdateUserApproval(request);
                        break;
                    case RequestTypeConstant.Delete:
                        await ProcessDeleteUserApproval(request);
                        break;
                }

                // Update request status to approved
                request.Status = RequestStatusConstant.Authorised;
                request.CheckerId = approver;
                request.ApproveDate = DateTime.UtcNow;

                await _userRequestRepository.UpdateAsync(request);
                await _userRequestRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            // ===== Return result =====
            return true;
        }

        /// <summary>
        /// Reject pending user request by ID.
        /// </summary>
        public async Task<bool> RejectPendingUserRequestAsync(long requestId, string reason)
        {
            // ===== Get request data =====
            var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                throw new ValidationException(ErrorCode.RequestNotFound);
            }

            if (request.Status != RequestStatusConstant.Unauthorised)
            {
                throw new ValidationException(ErrorCode.RequestNotPending);
            }

            // ===== Prepare data =====
            var rejecter = _userService.GetCurrentUsername() ?? "system";

            // ===== Process rejection with transaction =====
            await using var transaction = await _userRequestRepository.BeginTransactionAsync();
            try
            {
                // Revert status user
                if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) && request.EntityId > 0)
                {
                    var user = await _userRepository.FindByCondition(x => x.Id == request.EntityId).FirstOrDefaultAsync();

                    if (user != null) 
                    {
                        user.Status = RequestStatusConstant.Authorised;
                        await _userRepository.UpdateAsync(user);
                    }
                }

                // Reject request
                request.Status = RequestStatusConstant.Rejected;
                request.CheckerId = rejecter;
                request.ApproveDate = DateTime.UtcNow;
                request.Comments = reason;

                await _userRequestRepository.UpdateAsync(request);

                // Save changes through unit of work
                await _userRequestRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to reject user request ID '{requestId}'.");
            }

            // ===== Return result =====
            return true;
        }

        #endregion

        #region Process Functions

        private static void ConvertCreateUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
        {
            var data = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);

            if(data == null)
            {
                throw new ValidationException(ErrorCode.InvalidRequestData);
            }

            result.NewData = new UserRequestDataDto
            {
                UserName = data.UserName,
                FullName = data.FullName,
                Email = data.Email,
                BranchId = data.BranchId
            };
        }

        private async Task ProcessUpdateUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
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
                        result.OldData = new UserRequestDataDto
                        {
                            UserName = currentUser.UserName ?? string.Empty,
                            FullName = currentUser.FullName,
                            Email = currentUser.Email,
                            BranchId = currentUser.BranchId
                        };
                    }
                }

                result.NewData = new UserRequestDataDto
                {
                    UserName = data?.GetValueOrDefault("UserName")?.ToString() ?? string.Empty,
                    FullName = data?.GetValueOrDefault("FullName")?.ToString(),
                    Email = data?.GetValueOrDefault("Email")?.ToString(),
                    BranchId = data?.GetValueOrDefault("BranchId")?.ToString() != null ? long.Parse(data["BranchId"].ToString()!) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process update user request data for request {RequestId}", request.Id);
            }
        }

        private async Task ProcessDeleteUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
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
                        result.OldData = new UserRequestDataDto
                        {
                            UserName = currentUser.UserName ?? string.Empty,
                            FullName = currentUser.FullName,
                            Email = currentUser.Email,
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
            // ===== Validate =====
            if (string.IsNullOrEmpty(request.RequestedData))
            {
                throw new Exception("Request data is empty for CREATE request.");
            }

            var dto = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
            if (dto == null)
            {
                throw new Exception("Invalid CREATE request data format.");
            }

            var isValid = await ValidateCreateUserRequestAsync(dto, request.Id);
            if (!isValid)
            {
                throw new ValidationException(ErrorCode.InvalidRequest);
            }

            // ===== Create new user =====
            var newUser = new User
            {
                UserName = dto.UserName,
                NormalizedUserName = dto.UserName.ToUpperInvariant(),
                Email = dto.Email,
                NormalizedEmail = dto.Email?.ToUpperInvariant(),
                FullName = dto.FullName,
                BranchId = dto.BranchId,
                IsActive = dto.IsActive,
                Status = RequestStatusConstant.Authorised,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // ===== Hash password and create user =====
            var tempPassword = "TempPassword123!";
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, tempPassword);

            // ===== Create user using repository (no transaction - handled by caller) =====
            await _userRepository.CreateAsync(newUser);

            return newUser.Id;
        }

        private async Task<long> ProcessUpdateUserApproval(UserRequest request)
        {
            if (string.IsNullOrEmpty(request.RequestedData))
            {
                throw new Exception("Request data is empty for UPDATE request.");
            }

            var dto = JsonSerializer.Deserialize<UpdateUserRequest>(request.RequestedData);
            if (dto == null)
            {
                throw new ValidationException(ErrorCode.InvalidRequestData);
            }

            // ===== Find current user =====
            var user = await _userRepository.FindByCondition(u => u.UserName == dto.UserName)
                                            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ValidationException(ErrorCode.UserNotFound);
            }

            // ===== Apply updates =====
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.BranchId = dto.BranchId;
            user.IsActive = dto.IsActive;
            user.Status = RequestStatusConstant.Authorised;

            // ===== Update user (no transaction - handled by caller) =====
            await _userRepository.UpdateAsync(user);

            // ===== Update request status =====
            request.Status = RequestStatusConstant.Authorised;
            request.EntityId = user.Id;
            request.CheckerId = _userService.GetCurrentUsername() ?? "system";
            request.ApproveDate = DateTime.UtcNow;

            await _userRequestRepository.UpdateAsync(request);

            return user.Id;
        }

        private async Task<long> ProcessDeleteUserApproval(UserRequest request)
        {
            if (string.IsNullOrEmpty(request.RequestedData))
            {
                throw new Exception("Request data is empty for DELETE request.");
            }

            var dto = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
            if (dto == null)
            {
                throw new ValidationException(ErrorCode.InvalidRequestData);
            }

            var userName = dto.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ValidationException(ErrorCode.UserNameRequired);
            }

            // ===== Find user =====
            var user = await _userRepository.FindByCondition(u => u.UserName == userName)
                                            .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ValidationException(ErrorCode.UserNotFound);
            }

            // ===== Soft delete user =====
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            user.Status = RequestStatusConstant.Authorised;

            // ===== Update user (no transaction - handled by caller) =====
            await _userRepository.UpdateAsync(user);

            // ===== Update request status =====
            request.Status = RequestStatusConstant.Authorised;
            request.EntityId = user.Id;
            request.CheckerId = _userService.GetCurrentUsername() ?? "system";
            request.ApproveDate = DateTime.UtcNow;

            await _userRequestRepository.UpdateAsync(request);

            return user.Id;
        }

        private async Task<bool> ValidateCreateUserRequestAsync(CreateUserRequest request, long? excludeRequestId = null)
        {
            var username = request.UserName.ToLower();
            var email = request.Email.ToLower();

            // ===== Validate request =====
            // Check if user already exists by username
            if (await _userRepository.ExistsByUserNameAsync(username))
            {
                throw new ValidationException(ErrorCode.UserAlreadyExists);
            }

            // Check if user already exists by email
            if (await _userRepository.ExistsByEmailAsync(email))
            {
                throw new ValidationException(ErrorCode.EmailAlreadyExists);
            }

            // Check if user request already exists with pending status by username (excluding current request)
            if (await _userRequestRepository.ExistsPendingByUserNameAsync(username, excludeRequestId))
            {
                throw new ValidationException(ErrorCode.UserRequestExists);
            }

            // Check if user request already exists with pending status by email (excluding current request)
            if (await _userRequestRepository.ExistsPendingByEmailAsync(email, excludeRequestId))
            {
                throw new ValidationException(ErrorCode.EmailAlreadyExists);
            }

            return true;
        }

        #endregion
    }
}