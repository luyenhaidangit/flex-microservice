using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Grpc.Configurations
{
    public class GrpcClientSettings
    {
        [Required]
        public string BranchService { get; set; } = string.Empty;
    }
}
