using FluentValidation;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Validators.User
{
    public class CreateUserDirectRequestValidator : AbstractValidator<CreateUserDirectRequest>
    {
        public CreateUserDirectRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(1, 250).WithMessage("Full name must be between 1 and 250 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .Length(1, 256).WithMessage("Email must be between 1 and 256 characters");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch ID must be greater than 0");
        }
    }
}
