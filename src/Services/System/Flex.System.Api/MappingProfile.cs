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
        public static CreateBranchRequest ParseProposedData(this BranchRequest branchRequest)
        {
            try
            {
                var data = JsonSerializer.Deserialize<CreateBranchRequest>(branchRequest.ProposedData);
                if (data == null || string.IsNullOrEmpty(data.Code))
                    throw new ArgumentException("Thông tin chi nhánh không đầy đủ.");

                return data;
            }
            catch (JsonException)
            {
                throw new ArgumentException("Dữ liệu đề xuất không hợp lệ.");
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
