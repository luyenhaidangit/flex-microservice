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
