using Flex.Shared.DTOs.System.Branch;
using System.Text.Json;
using Flex.System.Api.Entities;
using Flex.Shared.Constants.Common;
using Flex.Shared.Constants.System.Branch;

namespace Flex.System.Api
{
    public static class MappingProfile
    {
        // Branch
        public static BranchRequestHeader MapToBranchRequestHeader(CreateBranchRequest request)
        {
            return new BranchRequestHeader
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised,
                RequestedDate = DateTime.UtcNow,
                Comments = request.Comments
            };
        }

        public static BranchRequestData MapToBranchRequestData(CreateBranchRequest request, long requestId)
        {
            return new BranchRequestData
            {
                RequestId = requestId,
                Code = request.Code,
                Name = request.Name,
                Address = request.Address
            };
        }

        public static BranchMaster MapToBranchMaster(BranchRequestData data)
        {
            return new BranchMaster
            {
                Code = data.Code,
                Name = data.Name,
                Address = data.Address,
                Status = BranchStatusConstant.Active
            };
        }

        public static BranchAuditLog MapToCreateAuditLog(BranchMaster master, string requestedBy, string approvedBy)
        {
            return new BranchAuditLog
            {
                EntityId = master.Id,
                Operation = AuditOperationConstant.Create,
                OldValue = null,
                NewValue = JsonSerializer.Serialize(master),
                RequestedBy = requestedBy,
                ApproveBy = approvedBy,
                LogDate = DateTime.UtcNow
            };
        }

        public static BranchAuditLog MapToRejectAuditLog(long requestId, string requestedBy, string rejectedBy, string? reason = null)
        {
            return new BranchAuditLog
            {
                EntityId = requestId,
                Operation = AuditOperationConstant.Reject,
                OldValue = null,
                NewValue = reason,
                RequestedBy = requestedBy,
                ApproveBy = rejectedBy,
                LogDate = DateTime.UtcNow
            };
        }
    }
}
