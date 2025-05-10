using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Models.Requests
{
    public class CreateUserRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public long BranchId { get; set; }
    }
}
