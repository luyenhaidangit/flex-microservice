using Flex.Shared.DTOs.System.Branch;
using System.Text.Json;
using Flex.System.Api.Entities;
using Flex.Shared.Constants.Common;
using Flex.Shared.Constants.System.Branch;

namespace Flex.System.Api
{
    public static class MappingProfile
    {
        // ========== BRANCH ==========
        // 1. CreateBranchRequest -> BranchRequestHeader
        public static BranchRequestHeader MapToBranchRequestHeader(CreateBranchRequest request)
        {
            return new BranchRequestHeader
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised,
                RequestedDate = DateTime.UtcNow
            };
        }

        // 2. UpdateBranchRequest -> BranchRequestHeader
        public static BranchRequestHeader MapToBranchRequestHeader(UpdateBranchRequest request)
        {
            return new BranchRequestHeader
            {
                Action = RequestTypeConstant.Update,
                Status = RequestStatusConstant.Unauthorised,
                RequestedDate = DateTime.UtcNow
            };
        }

        // 3. CreateBranchRequest -> BranchRequestData
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

        // 4. CreateBranchRequest -> BranchRequestData
        public static BranchRequestData MapToBranchRequestData(UpdateBranchRequest request, long requestId)
        {
            return new BranchRequestData
            {
                RequestId = requestId,
                Code = request.Code,
                Name = request.Name,
                Address = request.Address
            };
        }

        // 5. BranchMaster -> BranchMaster
        public static BranchMaster CloneBranchMaster(BranchMaster master)
        {
            return new BranchMaster
            {
                Id = master.Id,
                Code = master.Code,
                Name = master.Name,
                Address = master.Address
            };
        }

        public static BranchRequestData MapToBranchRequestData(DeleteBranchRequest request, long requestId)
        {
            return new BranchRequestData
            {
                RequestId = requestId,
                Code = request.Code,
                Name = request.Name,    // Add this field to DeleteBranchRequest
                Address = request.Address // Add this field to DeleteBranchRequest  
            };
        }

        public static BranchRequestHeader MapToBranchRequestHeader(DeleteBranchRequest request)
        {
            return new BranchRequestHeader
            {
                Action = RequestTypeConstant.Delete,
                Status = RequestStatusConstant.Unauthorised,
                RequestedDate = DateTime.UtcNow
            };
        }
        public static BranchMaster MapToBranchMaster(BranchRequestData data)
        {
            return new BranchMaster
            {
                Code = data.Code,
                Name = data.Name,
                Address = data.Address
            };
        }

        public static BranchAuditLog MapToAuditLog(long entityId,string operation,object? oldValue,object? newValue,string requestedBy,string approvedBy)
        {
            return new BranchAuditLog
            {
                EntityId = entityId,
                Operation = operation,
                OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
                NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
                RequestedBy = requestedBy,
                ApproveBy = approvedBy,
                LogDate = DateTime.UtcNow
            };
        }
    }
}
