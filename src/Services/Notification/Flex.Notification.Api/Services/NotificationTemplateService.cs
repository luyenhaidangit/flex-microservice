using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly ILogger<NotificationTemplateService> _logger;
        private readonly INotificationTemplateRepository _notificationTemplateRepository;
        //private readonly IUserRequestRepository _userRequestRepository;
        //private readonly ICurrentUserService _userService;
        //private readonly IBranchIntegrationService _branchIntegrationService;
        //private readonly IPasswordHasher<User> _passwordHasher;
        //private readonly IPasswordGenerationService _passwordGenerationService;
        //private readonly IUserNotificationService _userNotificationService;

        public NotificationTemplateService(
            ILogger<NotificationTemplateService> logger,
            INotificationTemplateRepository notificationTemplateRepository
            //IdentityDbContext dbContext,
            //ICurrentUserService userService,
            //IUserRequestRepository userRequestRepository,
            //IBranchIntegrationService branchIntegrationService,
            //IPasswordHasher<User> passwordHasher,
            //IPasswordGenerationService passwordGenerationService,
            //IUserNotificationService userNotificationService
            )
        {
            _logger = logger;
            _notificationTemplateRepository = notificationTemplateRepository;
            //_dbContext = dbContext;
            //_userService = userService;
            //_userRepository = userRepository;
            //_userRequestRepository = userRequestRepository;
            //_branchIntegrationService = branchIntegrationService;
            //_passwordHasher = passwordHasher;
            //_passwordGenerationService = passwordGenerationService;
            //_userNotificationService = userNotificationService;
        }

        //#region Query

        /// <summary>
        /// Get all approved notification templates with pagination.
        /// </summary>
        public async Task<PagedResult<NotificationTemplatePagingDto>> GetNotificationTemplatesPagedAsync(GetNotificationTemplatesPagingRequest request, CancellationToken ct)
        {
            // ===== Process request parameters =====
            var keyword = request.Keyword?.Trim().ToLower();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            // ===== Build query =====
            var query = _notificationTemplateRepository.FindAll().AsNoTracking()
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    t => EF.Functions.Like((t.TemplateKey ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((t.Name ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((t.Subject ?? string.Empty).ToLower(), $"%{keyword}%"));

            // ===== Execute query =====
            var total = await query.CountAsync(ct);
            var raw = await query
                .OrderBy(t => t.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new { 
                    t.TemplateKey, 
                    t.Name, 
                    t.Channel, 
                    t.Format, 
                    t.Language, 
                    t.Subject, 
                    t.BodyHtml, 
                    t.BodyText, 
                    t.IsActive, 
                    t.VariablesSpecJson 
                })
                .ToListAsync(ct);

            // ===== Build result =====
            var items = raw.Select(t => new NotificationTemplatePagingDto
            {
                TemplateKey = t.TemplateKey,
                Name = t.Name,
                Channel = t.Channel,
                Format = t.Format,
                Language = t.Language,
                Subject = t.Subject,
                BodyHtml = t.BodyHtml,
                BodyText = t.BodyText,
                IsActive = t.IsActive,
                VariablesSpecJson = t.VariablesSpecJson
            }).ToList();

            // ===== Return result =====
            return PagedResult<NotificationTemplatePagingDto>.Create(pageIndex, pageSize, total, items);
        }

        ///// <summary>
        ///// Get approved user by username.
        ///// </summary>
        //public async Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken ct)
        //{
        //    // ===== Find user by username =====
        //    var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower(), ct);

        //    if (user == null)
        //    {
        //        throw new Exception($"User '{userName}' not found.");
        //    }
        //    ;

        //    // ===== Get branch information =====
        //    var branch = new BranchLookupDto(0, "");
        //    if (user.BranchId > 0)
        //    {
        //        try
        //        {
        //            branch = await _branchIntegrationService.GetBranchByIdAsync(user.BranchId, ct) ?? new BranchLookupDto(0, "");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogWarning(ex, "Failed to retrieve branch information for user {UserName} with BranchId {BranchId}", userName, user.BranchId);
        //            throw;
        //        }
        //    }

        //    // ===== Get user roles =====
        //    var roles = await _dbContext.Set<UserRole>()
        //        .Where(ur => ur.UserId == user.Id)
        //        .Join(_dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r.Code)
        //        .ToListAsync(ct);

        //    // ===== Return result =====
        //    return new UserDetailDto
        //    {
        //        UserName = user.UserName ?? string.Empty,
        //        FullName = user.FullName,
        //        Email = user.Email,
        //        BranchName = branch.Name,
        //        IsActive = user.IsActive,
        //        Roles = roles,
        //        Branch = branch ?? new BranchLookupDto(0, "")
        //    };
        //}

        ///// <summary>
        ///// Get user change history by username.
        ///// </summary>
        //public async Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName)
        //{
        //    // ===== Find user with username =====
        //    var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower());

        //    if (user == null)
        //    {
        //        throw new Exception($"User with username '{userName}' not exists.");
        //    }

        //    // ===== Get user histories by user Id =====
        //    var userId = user.Id;

        //    var requests = await _userRequestRepository.FindByCondition(r => r.EntityId == userId)
        //        .AsNoTracking()
        //        .OrderByDescending(r => r.RequestedDate)
        //        .Select(r => new
        //        {
        //            r.Id,
        //            r.MakerId,
        //            r.RequestedDate,
        //            r.CheckerId,
        //            r.ApproveDate,
        //            r.Status,
        //            r.Comments,
        //            r.RequestedData
        //        })
        //        .AsNoTracking()
        //        .ToListAsync();

        //    // ===== Build result =====
        //    var historyItems = requests.Select((req, idx) => new UserChangeHistoryDto
        //    {
        //        Id = req.Id,
        //        MakerBy = req.MakerId,
        //        MakerTime = req.RequestedDate,
        //        ApproverBy = req.CheckerId,
        //        ApproverTime = req.ApproveDate,
        //        Status = req.Status,
        //        Description = req.Comments,
        //        Changes = req.RequestedData
        //    }).ToList();

        //    return historyItems;
        //}

        ///// <summary>
        ///// Get all pending user requests with pagination.
        ///// </summary>
        //public async Task<PagedResult<UserPendingPagingDto>> GetPendingUserRequestsPagedAsync(GetUserRequestsPagingRequest request, CancellationToken ct)
        //{
        //    // ===== Process request parameters =====
        //    var keyword = request.Keyword?.Trim().ToLower();
        //    var requestType = request.Type?.Trim().ToUpperInvariant();
        //    int pageIndex = request.PageIndexValue;
        //    int pageSize = request.PageSizeValue;

        //    // ===== Build query using view =====
        //    var pendingQuery = _userRequestRepository.GetAllUserRequests()
        //        .WhereIf(!string.IsNullOrEmpty(keyword),
        //            r => EF.Functions.Like(r.UserName.ToLower(), $"%{keyword}%") ||
        //                 EF.Functions.Like(r.FullName.ToLower(), $"%{keyword}%") ||
        //                 EF.Functions.Like(r.Email.ToLower(), $"%{keyword}%"))
        //        .Where(x => x.Status == RequestStatusConstant.Unauthorised)
        //        .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestTypeConstant.All,
        //            r => r.Action == requestType)
        //        .AsNoTracking()
        //        .Select(r => new UserPendingPagingDto
        //        {
        //            RequestId = r.RequestId,
        //            Action = r.Action,
        //            RequestedBy = r.RequestedBy,
        //            RequestedDate = r.RequestedDate,
        //            UserName = r.UserName,
        //            FullName = r.FullName,
        //            Email = r.Email
        //        });

        //    // ===== Execute query =====
        //    var total = await pendingQuery.CountAsync(ct);
        //    var items = await pendingQuery
        //        .OrderByDescending(dto => dto.RequestedDate)
        //        .ThenBy(dto => dto.RequestId)
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync(ct);

        //    // ===== Return result =====
        //    return PagedResult<UserPendingPagingDto>.Create(pageIndex, pageSize, total, items);
        //}

        ///// <summary>
        ///// Get pending user request detail by request ID.
        ///// </summary>
        //public async Task<PendingRequestDtoBase<UserRequestDataDto>> GetPendingUserRequestByIdAsync(long requestId)
        //{
        //    // ===== Get request data =====
        //    var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();

        //    if (request == null)
        //    {
        //        throw new ValidationException(ErrorCode.RequestNotFound);
        //    }

        //    // ===== Build base result =====
        //    var result = new PendingRequestDtoBase<UserRequestDataDto>
        //    {
        //        RequestId = request.Id.ToString(),
        //        Type = request.Action,
        //        CreatedBy = request.MakerId.ToString(),
        //        CreatedDate = request.RequestedDate.ToString("yyyy-MM-dd HH:mm:ss")
        //    };

        //    // ===== Process request data based on action type =====
        //    switch (request.Action)
        //    {
        //        case RequestTypeConstant.Create:
        //            await ConvertCreateUserRequestData(request, result);
        //            break;
        //        case RequestTypeConstant.Update:
        //            await ConvertUpdateUserRequestData(request, result);
        //            break;
        //        case RequestTypeConstant.Delete:
        //            await ConvertDeleteUserRequestData(request, result);
        //            break;
        //        default:
        //            throw new ValidationException(ErrorCode.InvalidRequestType);
        //    }

        //    return result;
        //}
        //#endregion

        //#region Command

        ///// <summary>
        ///// Create user request (requires approval).
        ///// </summary>
        //public async Task<long> CreateUserRequestAsync(CreateUserRequest request)
        //{
        //    // ===== Validate =====
        //    var isValid = await ValidateCreateUserRequestAsync(request);
        //    if (!isValid)
        //    {
        //        throw new ValidationException(ErrorCode.InvalidRequest);
        //    }

        //    // ===== Process =====
        //    var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
        //    var requestedJson = JsonSerializer.Serialize(request);
        //    var requestDto = new UserRequest
        //    {
        //        Action = RequestTypeConstant.Create,
        //        Status = RequestStatusConstant.Unauthorised,
        //        EntityId = 0,
        //        MakerId = requestedBy,
        //        RequestedDate = DateTime.UtcNow,
        //        Comments = "Yêu cầu thêm mới người sử dụng.",
        //        RequestedData = requestedJson
        //    };
        //    await _userRequestRepository.CreateAsync(requestDto);

        //    return requestDto.Id;
        //}

        ///// <summary>
        ///// Create update user request.
        ///// </summary>
        //public async Task<long> UpdateUserRequestAsync(UpdateUserRequest request)
        //{
        //    var email = request.Email?.ToLower();

        //    // ===== Validation =====
        //    // ===== Check user exists =====
        //    var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName);
        //    if (user == null)
        //    {
        //        throw new Exception($"User with username '{request.UserName}' does not exist.");
        //    }

        //    // ===== Check email uniqueness (if email is being changed) =====
        //    if (!string.IsNullOrEmpty(email) && user.Email?.ToLower() != email)
        //    {
        //        if (await _userRepository.ExistsByEmailAsync(email))
        //        {
        //            throw new ValidationException(ErrorCode.EmailAlreadyExists);
        //        }
        //    }

        //    // ===== Check user request exists =====
        //    if (user.Status == RequestStatusConstant.Unauthorised)
        //    {
        //        throw new Exception("A pending update request already exists for this user.");
        //    }

        //    // ===== Process =====
        //    // ===== Create update user request =====
        //    var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
        //    var requestedJson = JsonSerializer.Serialize(request);
        //    var userRequest = new UserRequest
        //    {
        //        Action = RequestTypeConstant.Update,
        //        Status = RequestStatusConstant.Unauthorised,
        //        EntityId = user.Id,
        //        MakerId = requestedBy,
        //        RequestedDate = DateTime.UtcNow,
        //        RequestedData = requestedJson,
        //        //Comments = request.Comment ?? "Yêu cầu cập nhật người dùng."
        //    };

        //    // ===== Update status process user =====
        //    user.Status = RequestStatusConstant.Unauthorised;

        //    // ===== Transaction =====
        //    await using var transaction = await _userRequestRepository.BeginTransactionAsync();
        //    try
        //    {
        //        await _userRequestRepository.CreateAsync(userRequest);
        //        _dbContext.Update(user);
        //        await _dbContext.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw new Exception("Failed to create update user request.");
        //    }

        //    return userRequest.Id;
        //}

        ///// <summary>
        ///// Delete user immediately.
        ///// </summary>
        //public async Task<long> DeleteUserRequestAsync(string userName)
        //{
        //    // ===== Validate =====
        //    var isValid = await ValidateDeleteUserRequestAsync(userName);
        //    if (!isValid)
        //    {
        //        throw new ValidationException(ErrorCode.InvalidRequest);
        //    }

        //    var user = await _userRepository.FindByCondition(x => x.UserName == userName).FirstOrDefaultAsync();
        //    if (user == null)
        //    {
        //        throw new ValidationException(ErrorCode.UserNotFound);
        //    }

        //    // ===== Check pending request =====
        //    if (user.Status == RequestStatusConstant.Unauthorised)
        //    {
        //        throw new Exception("A pending delete request already exists for this user.");
        //    }

        //    // ===== Create snapshot =====
        //    var currentSnapshot = new UserDetailDto
        //    {
        //        UserName = user.UserName ?? string.Empty,
        //        FullName = user.FullName,
        //        Email = user.Email,
        //        BranchName = "",
        //        IsActive = user.IsActive
        //    };

        //    var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
        //    var request = new UserRequest
        //    {
        //        Action = RequestTypeConstant.Delete,
        //        Status = RequestStatusConstant.Unauthorised,
        //        EntityId = user.Id,
        //        MakerId = requestedBy,
        //        RequestedDate = DateTime.UtcNow,
        //        RequestedData = JsonSerializer.Serialize(currentSnapshot),
        //        Comments = "Yêu cầu xóa người dùng."
        //    };

        //    // ===== Update user status =====
        //    user.Status = RequestStatusConstant.Unauthorised;

        //    // ===== Transaction =====
        //    await using var transaction = await _userRequestRepository.BeginTransactionAsync();
        //    try
        //    {
        //        await _userRequestRepository.CreateAsync(request);
        //        await _userRepository.UpdateAsync(user);

        //        await _dbContext.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }

        //    return request.Id;
        //}

        ///// <summary>
        ///// Approve pending user request by ID.
        ///// </summary>
        //public async Task<bool> ApprovePendingUserRequestAsync(long requestId)
        //{
        //    // ===== Get request data =====
        //    var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
        //        .FirstOrDefaultAsync();

        //    if (request == null)
        //    {
        //        throw new ValidationException(ErrorCode.RequestNotFound);
        //    }

        //    if (request.Status != RequestStatusConstant.Unauthorised)
        //    {
        //        throw new ValidationException(ErrorCode.RequestNotPending);
        //    }

        //    // ===== Process approval with transaction =====
        //    var approver = _userService.GetCurrentUsername() ?? "system";

        //    await using var transaction = await _userRequestRepository.BeginTransactionAsync();
        //    try
        //    {
        //        // Process approval based on action type
        //        switch (request.Action)
        //        {
        //            case RequestTypeConstant.Create:
        //                await ProcessApproveCreateUser(request);
        //                break;
        //            case RequestTypeConstant.Update:
        //                await ProcessApproveUpdateUser(request);
        //                break;
        //            case RequestTypeConstant.Delete:
        //                await ProcessApproveDeleteUser(request);
        //                break;
        //        }

        //        // Update request status to approved
        //        request.Status = RequestStatusConstant.Authorised;
        //        request.CheckerId = approver;
        //        request.ApproveDate = DateTime.UtcNow;

        //        await _userRequestRepository.UpdateAsync(request);
        //        await _userRequestRepository.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }

        //    // ===== Return result =====
        //    return true;
        //}

        ///// <summary>
        ///// Reject pending user request by ID.
        ///// </summary>
        //public async Task<bool> RejectPendingUserRequestAsync(long requestId, string reason)
        //{
        //    // ===== Get request data =====
        //    var request = await _userRequestRepository.FindByCondition(r => r.Id == requestId)
        //        .FirstOrDefaultAsync();

        //    if (request == null)
        //    {
        //        _logger.LogWarning("Attempted to reject non-existent request ID: {RequestId}", requestId);
        //        throw new ValidationException(ErrorCode.RequestNotFound);
        //    }

        //    if (request.Status != RequestStatusConstant.Unauthorised)
        //    {
        //        _logger.LogWarning("Attempted to reject request ID: {RequestId} with status: {Status}", requestId, request.Status);
        //        throw new ValidationException(ErrorCode.RequestNotPending);
        //    }

        //    // ===== Prepare data =====
        //    var rejecter = _userService.GetCurrentUsername() ?? "system";

        //    // ===== Process rejection with transaction =====
        //    await using var transaction = await _userRequestRepository.BeginTransactionAsync();
        //    try
        //    {
        //        // ===== Handle different request types =====
        //        if (request.Action == RequestTypeConstant.Create)
        //        {
        //            // For CREATE requests, user doesn't exist yet, so no need to revert anything
        //            _logger.LogInformation("Rejecting CREATE request ID: {RequestId}, UserName: {UserName}, " +
        //                "RejectedBy: {RejectedBy}, Reason: {Reason}", requestId, GetUserNameFromRequestData(request), rejecter, reason);
        //        }
        //        else if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) && request.EntityId > 0)
        //        {
        //            // For UPDATE/DELETE requests, revert user status
        //            var user = await _userRepository.FindByCondition(x => x.Id == request.EntityId).FirstOrDefaultAsync();

        //            if (user != null)
        //            {
        //                user.Status = RequestStatusConstant.Authorised;
        //                await _userRepository.UpdateAsync(user);

        //                _logger.LogInformation("Reverted user status to Authorised for UserId: {UserId}, UserName: {UserName} " +
        //                    "due to rejected {Action} request ID: {RequestId}", user.Id, user.UserName, request.Action, requestId);
        //            }
        //            else
        //            {
        //                _logger.LogWarning("User not found for EntityId: {EntityId} in rejected request ID: {RequestId}",
        //                    request.EntityId, requestId);
        //            }
        //        }

        //        // ===== Update request status =====
        //        request.Status = RequestStatusConstant.Rejected;
        //        request.CheckerId = rejecter;
        //        request.ApproveDate = DateTime.UtcNow;
        //        request.Comments = reason;

        //        await _userRequestRepository.UpdateAsync(request);

        //        // ===== Save changes through unit of work =====
        //        await _userRequestRepository.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        // ===== Audit logging =====
        //        _logger.LogInformation("User request rejected successfully - RequestId: {RequestId}, Action: {Action}, " +
        //            "MakerId: {MakerId}, RejectedBy: {RejectedBy}, Reason: {Reason}",
        //            requestId, request.Action, request.MakerId, rejecter, reason);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Failed to reject user request ID: {RequestId}, RejectedBy: {RejectedBy}", requestId, rejecter);
        //        throw new Exception($"Failed to reject user request ID '{requestId}'.");
        //    }

        //    // ===== Return result =====
        //    return true;
        //}

        ///// <summary>
        ///// Change user password (required on first login).
        ///// </summary>
        //public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        //{
        //    // ===== Validate request parameters =====
        //    if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.CurrentPassword) ||
        //        string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
        //    {
        //        _logger.LogWarning("Invalid password change request - missing required fields for UserName: {UserName}", request.UserName);
        //        throw new ValidationException(ErrorCode.InvalidRequest);
        //    }

        //    if (request.NewPassword != request.ConfirmPassword)
        //    {
        //        _logger.LogWarning("Password mismatch for user: {UserName}", request.UserName);
        //        throw new ValidationException(ErrorCode.PasswordMismatch);
        //    }

        //    // ===== Find user =====
        //    var user = await _userRepository.FindByCondition(u => u.UserName == request.UserName)
        //                                    .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        _logger.LogWarning("User not found for password change: {UserName}", request.UserName);
        //        throw new ValidationException(ErrorCode.UserNotFound);
        //    }

        //    // ===== Verify current password =====
        //    var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.CurrentPassword);
        //    if (passwordVerificationResult == PasswordVerificationResult.Failed)
        //    {
        //        _logger.LogWarning("Invalid current password for user: {UserName}", request.UserName);
        //        throw new ValidationException(ErrorCode.InvalidCurrentPassword);
        //    }

        //    // ===== Additional validation: Check if password change is actually required =====
        //    if (!user.PasswordChangeRequired)
        //    {
        //        _logger.LogWarning("Password change not required for user: {UserName} - this might be a security concern", request.UserName);
        //        // Note: We still allow the password change, but log it as a warning
        //    }

        //    // ===== Update password with transaction =====
        //    await using var transaction = await _userRepository.BeginTransactionAsync();
        //    try
        //    {
        //        // ===== Hash new password =====
        //        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        //        user.PasswordChangeRequired = false; // Clear the password change requirement
        //        user.SecurityStamp = Guid.NewGuid().ToString(); // Update security stamp
        //        user.ConcurrencyStamp = Guid.NewGuid().ToString(); // Update concurrency stamp

        //        await _userRepository.UpdateAsync(user);
        //        await _userRepository.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        // ===== Audit logging =====
        //        _logger.LogInformation("Password changed successfully for user {UserName} (UserId: {UserId})",
        //            request.UserName, user.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Failed to change password for user {UserName} (UserId: {UserId})",
        //            request.UserName, user.Id);
        //        throw new Exception($"Failed to change password for user '{request.UserName}'.");
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Check if user needs to change password on first login.
        ///// </summary>
        //public async Task<bool> CheckPasswordChangeRequiredAsync(string userName)
        //{
        //    var user = await _userRepository.FindByCondition(u => u.UserName == userName)
        //                                    .AsNoTracking()
        //                                    .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        throw new ValidationException(ErrorCode.UserNotFound);
        //    }

        //    return user.PasswordChangeRequired;
        //}

        //#endregion

        //#region Helper Methods

        ///// <summary>
        ///// Extract username from request data for logging purposes
        ///// </summary>
        //private string GetUserNameFromRequestData(UserRequest request)
        //{
        //    try
        //    {
        //        if (request.Action == RequestTypeConstant.Create)
        //        {
        //            var dto = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
        //            return dto?.UserName ?? "Unknown";
        //        }
        //        else if (request.Action == RequestTypeConstant.Update)
        //        {
        //            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData);
        //            return data?.GetValueOrDefault("UserName")?.ToString() ?? "Unknown";
        //        }
        //        else if (request.Action == RequestTypeConstant.Delete)
        //        {
        //            // For DELETE, we can get username from the user entity
        //            return "Unknown"; // Will be logged separately if needed
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning(ex, "Failed to extract username from request data for request ID: {RequestId}", request.Id);
        //    }

        //    return "Unknown";
        //}

        //#endregion

        //#region Process Functions

        //private async Task ConvertCreateUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
        //{
        //    var data = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);

        //    if (data == null)
        //    {
        //        throw new ValidationException(ErrorCode.InvalidRequestData);
        //    }

        //    // ===== Get branch name =====
        //    string branchName = string.Empty;
        //    try
        //    {
        //        if (data.BranchId > 0)
        //        {
        //            var branch = await _branchIntegrationService.GetBranchByIdAsync(data.BranchId);
        //            branchName = branch?.Name ?? string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning(ex, "Failed to get branch name for BranchId: {BranchId}", data.BranchId);
        //    }

        //    result.NewData = new UserRequestDataDto
        //    {
        //        UserName = data.UserName,
        //        FullName = data.FullName,
        //        Email = data.Email,
        //        BranchId = data.BranchId,
        //        BranchName = branchName,
        //        IsActive = data.IsActive
        //    };
        //}

        //private async Task ConvertUpdateUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
        //{
        //    try
        //    {
        //        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData);
        //        var userName = data?.GetValueOrDefault("UserName")?.ToString();

        //        if (!string.IsNullOrEmpty(userName))
        //        {
        //            // ===== Get current user data =====
        //            var currentUser = await _userRepository.FindAll()
        //                .AsNoTracking()
        //                .FirstOrDefaultAsync(u => u.UserName == userName);

        //            if (currentUser != null)
        //            {
        //                // ===== Get branch name for old data =====
        //                string oldBranchName = string.Empty;
        //                try
        //                {
        //                    if (currentUser.BranchId > 0)
        //                    {
        //                        var oldBranch = await _branchIntegrationService.GetBranchByIdAsync(currentUser.BranchId);
        //                        oldBranchName = oldBranch?.Name ?? string.Empty;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogWarning(ex, "Failed to get branch name for old data BranchId: {BranchId}", currentUser.BranchId);
        //                }

        //                result.OldData = new UserRequestDataDto
        //                {
        //                    UserName = currentUser.UserName ?? string.Empty,
        //                    FullName = currentUser.FullName,
        //                    Email = currentUser.Email ?? string.Empty,
        //                    BranchId = currentUser.BranchId,
        //                    BranchName = oldBranchName
        //                };
        //            }
        //        }

        //        // ===== Get branch name for new data =====
        //        string newBranchName = string.Empty;
        //        long newBranchId = data?.GetValueOrDefault("BranchId")?.ToString() != null ? long.Parse(data["BranchId"].ToString()!) : 0;
        //        try
        //        {
        //            if (newBranchId > 0)
        //            {
        //                var newBranch = await _branchIntegrationService.GetBranchByIdAsync(newBranchId);
        //                newBranchName = newBranch?.Name ?? string.Empty;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogWarning(ex, "Failed to get branch name for new data BranchId: {BranchId}", newBranchId);
        //        }

        //        result.NewData = new UserRequestDataDto
        //        {
        //            UserName = data?.GetValueOrDefault("UserName")?.ToString() ?? string.Empty,
        //            FullName = data?.GetValueOrDefault("FullName")?.ToString(),
        //            Email = data?.GetValueOrDefault("Email")?.ToString(),
        //            BranchId = newBranchId,
        //            BranchName = newBranchName,
        //            IsActive = data?.GetValueOrDefault("IsActive") != null && bool.Parse(data["IsActive"].ToString()!)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to process update user request data for request {RequestId}", request.Id);
        //        throw;
        //    }
        //}

        //private async Task ConvertDeleteUserRequestData(UserRequest request, PendingRequestDtoBase<UserRequestDataDto> result)
        //{
        //    try
        //    {
        //        // ===== Get current user data by EntityId =====
        //        var currentUser = await _userRepository.FindByCondition(u => u.Id == request.EntityId)
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync();

        //        if (currentUser != null)
        //        {
        //            // ===== Get branch name for delete data =====
        //            string branchName = string.Empty;
        //            try
        //            {
        //                if (currentUser.BranchId > 0)
        //                {
        //                    var branch = await _branchIntegrationService.GetBranchByIdAsync(currentUser.BranchId);
        //                    branchName = branch?.Name ?? string.Empty;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogWarning(ex, "Failed to get branch name for delete data BranchId: {BranchId}", currentUser.BranchId);
        //            }

        //            result.OldData = new UserRequestDataDto
        //            {
        //                UserName = currentUser.UserName ?? string.Empty,
        //                FullName = currentUser.FullName,
        //                Email = currentUser.Email,
        //                BranchId = currentUser.BranchId,
        //                BranchName = branchName
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to process delete user request data for request {RequestId}", request.Id);
        //        throw;
        //    }
        //}

        //private async Task<long> ProcessApproveCreateUser(UserRequest request)
        //{
        //    // ===== Validate request data =====
        //    if (string.IsNullOrEmpty(request.RequestedData))
        //    {
        //        _logger.LogError("Request data is empty for CREATE request ID: {RequestId}", request.Id);
        //        throw new ValidationException(ErrorCode.InvalidRequestData);
        //    }

        //    var dto = JsonSerializer.Deserialize<CreateUserRequest>(request.RequestedData);
        //    if (dto == null)
        //    {
        //        _logger.LogError("Invalid CREATE request data format for request ID: {RequestId}", request.Id);
        //        throw new ValidationException(ErrorCode.InvalidRequestData);
        //    }

        //    // ===== Validate user data =====
        //    var isValid = await ValidateCreateUserRequestAsync(dto, request.Id);
        //    if (!isValid)
        //    {
        //        _logger.LogError("Validation failed for CREATE request ID: {RequestId}, UserName: {UserName}", request.Id, dto.UserName);
        //        throw new ValidationException(ErrorCode.InvalidRequest);
        //    }

        //    // ===== Create new user =====
        //    var newUser = new User
        //    {
        //        UserName = dto.UserName,
        //        NormalizedUserName = dto.UserName.ToUpperInvariant(),
        //        Email = dto.Email,
        //        NormalizedEmail = dto.Email?.ToUpperInvariant(),
        //        FullName = dto.FullName,
        //        BranchId = dto.BranchId,
        //        IsActive = dto.IsActive,
        //        Status = RequestStatusConstant.Authorised,
        //        EmailConfirmed = false,
        //        PhoneNumberConfirmed = false,
        //        TwoFactorEnabled = false,
        //        LockoutEnabled = true,
        //        AccessFailedCount = 0,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        ConcurrencyStamp = Guid.NewGuid().ToString(),
        //        PasswordChangeRequired = true
        //    };

        //    // ===== Generate random password and hash it =====
        //    var randomPassword = _passwordGenerationService.GenerateRandomPassword();
        //    newUser.PasswordHash = _passwordHasher.HashPassword(newUser, randomPassword);

        //    // ===== Create user using repository (no transaction - handled by caller) =====
        //    await _userRepository.CreateAsync(newUser);

        //    // ===== Send password notification email =====
        //    try
        //    {
        //        await _userNotificationService.SendPasswordNotificationAsync(
        //            dto.Email,
        //            dto.UserName,
        //            randomPassword,
        //            dto.FullName);

        //        _logger.LogInformation("Password notification email sent to {Email} for user {UserName} (Request ID: {RequestId})",
        //            dto.Email, dto.UserName, request.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to send password notification email to {Email} for user {UserName} (Request ID: {RequestId}). " +
        //            "User was created successfully but email notification failed.", dto.Email, dto.UserName, request.Id);
        //        // Don't throw exception here as user creation was successful
        //    }

        //    // ===== Audit logging =====
        //    _logger.LogInformation("User created successfully via approval process - UserName: {UserName}, UserId: {UserId}, " +
        //        "RequestId: {RequestId}, ApprovedBy: {ApprovedBy}, MakerId: {MakerId}",
        //        newUser.UserName, newUser.Id, request.Id, request.CheckerId, request.MakerId);

        //    return newUser.Id;
        //}

        //private async Task<long> ProcessApproveUpdateUser(UserRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.RequestedData))
        //    {
        //        throw new Exception("Request data is empty for UPDATE request.");
        //    }

        //    var dto = JsonSerializer.Deserialize<UpdateUserRequest>(request.RequestedData);
        //    if (dto == null)
        //    {
        //        throw new ValidationException(ErrorCode.InvalidRequestData);
        //    }

        //    // ===== Find current user =====
        //    var user = await _userRepository.FindByCondition(u => u.UserName == dto.UserName)
        //                                    .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        throw new ValidationException(ErrorCode.UserNotFound);
        //    }

        //    // ===== Apply updates =====
        //    user.FullName = dto.FullName;
        //    user.Email = dto.Email;
        //    user.BranchId = dto.BranchId;
        //    user.IsActive = dto.IsActive;
        //    user.Status = RequestStatusConstant.Authorised;

        //    // ===== Update user (no transaction - handled by caller) =====
        //    await _userRepository.UpdateAsync(user);

        //    // ===== Update request status =====
        //    request.Status = RequestStatusConstant.Authorised;
        //    request.EntityId = user.Id;
        //    request.CheckerId = _userService.GetCurrentUsername() ?? "system";
        //    request.ApproveDate = DateTime.UtcNow;

        //    await _userRequestRepository.UpdateAsync(request);

        //    return user.Id;
        //}

        //private async Task<long> ProcessApproveDeleteUser(UserRequest request)
        //{
        //    var idEntity = request.EntityId;

        //    // ===== Validate =====

        //    // ===== Find user =====
        //    var user = await _userRepository.FindByCondition(u => u.Id == idEntity)
        //                                    .FirstOrDefaultAsync();
        //    if (user == null)
        //    {
        //        throw new ValidationException(ErrorCode.UserNotFound);
        //    }

        //    // ===== Hard delete user =====
        //    await _userRepository.DeleteAsync(user);
        //    _logger.LogInformation("User deleted successfully: {UserId}", user.Id);

        //    // ===== Update request status =====
        //    request.Status = RequestStatusConstant.Authorised;
        //    request.EntityId = user.Id;
        //    request.CheckerId = _userService.GetCurrentUsername() ?? "system";
        //    request.ApproveDate = DateTime.UtcNow;

        //    await _userRequestRepository.UpdateAsync(request);
        //    _logger.LogInformation("User request updated to approved: {RequestId}", request.Id);

        //    return user.Id;
        //}

        //#endregion

        //#region Validate
        //private async Task<bool> ValidateCreateUserRequestAsync(CreateUserRequest request, long? excludeRequestId = null)
        //{
        //    var username = request.UserName.ToLower();
        //    var email = request.Email.ToLower();

        //    // ===== Validate request =====
        //    // Check if user already exists by username
        //    if (await _userRepository.ExistsByUserNameAsync(username))
        //    {
        //        throw new ValidationException(ErrorCode.UserAlreadyExists);
        //    }

        //    // Check if user already exists by email
        //    if (await _userRepository.ExistsByEmailAsync(email))
        //    {
        //        throw new ValidationException(ErrorCode.EmailAlreadyExists);
        //    }

        //    // Check if user request already exists with pending status by username (excluding current request)
        //    if (await _userRequestRepository.ExistsPendingByUserNameAsync(username, excludeRequestId))
        //    {
        //        throw new ValidationException(ErrorCode.UserRequestExists);
        //    }

        //    // Check if user request already exists with pending status by email (excluding current request)
        //    if (await _userRequestRepository.ExistsPendingByEmailAsync(email, excludeRequestId))
        //    {
        //        throw new ValidationException(ErrorCode.EmailAlreadyExists);
        //    }

        //    return true;
        //}

        //private Task<bool> ValidateDeleteUserRequestAsync(string idRequest)
        //{
        //    return Task.FromResult(true);
        //}
        //#endregion
    }
}
