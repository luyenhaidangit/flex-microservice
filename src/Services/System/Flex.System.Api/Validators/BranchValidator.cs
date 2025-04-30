using Flex.Shared.Constants.Common;
using Flex.Shared.DTOs.System.Branch;

namespace Flex.System.Api.Validators
{
    public static class BranchValidator
    {
        private static readonly string[] AllowedStatuses = new[]
        {
            StatusConstant.All,
            StatusConstant.Active,
            StatusConstant.Pending,
            StatusConstant.Canceled,
        };

        public static bool Validate(this GetBranchesPagingRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !AllowedStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                throw new BadHttpRequestException(
                    $"Invalid Status. Allowed values: {string.Join(", ", AllowedStatuses)}",
                    StatusCodes.Status400BadRequest
                );
            }

            return true;
        }
    }
}
