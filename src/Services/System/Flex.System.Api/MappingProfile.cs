using Flex.Shared.DTOs.System.Branch;
using System.Text.Json;
using Flex.System.Api.Entities;
using Flex.Shared.Constants.Common;

namespace Flex.System.Api
{
    public static class MappingProfile
    {
        // Branch
        public static BranchRequest ToCreateBranchRequest(this CreateBranchRequest dto)
        {
            return new BranchRequest
            {
                BranchId = null,
                RequestType = RequestTypeConstant.Create,
                ProposedData = JsonSerializer.Serialize(dto),
                Status = StatusConstant.Pending,
                RequestedBy = dto.RequestedBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static T? ParseProposedData<T>(this BranchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProposedData))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(request.ProposedData);
            }
            catch
            {
                return default;
            }
        }

        public static Branch ToBranch(this CreateBranchRequest request)
        {
            return new Branch
            {
                Code = request.Code,
                Name = request.Name,
                Address = request.Address,
                Region = request.Region,
                Manager = request.Manager,
                EstablishedDate = request.EstablishedDate,
                Status = StatusConstant.Active
            };
        }
    }
}
