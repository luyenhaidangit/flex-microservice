using Flex.Shared.DTOs.System.Branch;
using System.Text.Json;
using Flex.System.Api.Entities;
using Flex.Shared.Constants.Common;

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
                MakerId = request.MakerId,
                MakerDate = DateTime.UtcNow,
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
    }
}
