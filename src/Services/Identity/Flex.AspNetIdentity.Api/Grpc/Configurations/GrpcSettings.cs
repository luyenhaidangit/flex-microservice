using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Grpc.Configurations
{
    public class GrpcSettings
    {
        [Required]
        public string SystemUrl { get; set; } = string.Empty;
    }
}
