using FluentValidation;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Validators.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            // ===== UserName Validation =====
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches("^[A-Za-z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, hyphens and underscores")
                .Must(u => !u.StartsWith(".") && !u.StartsWith("_") && !u.StartsWith("-") && !u.EndsWith(".") && !u.EndsWith("_") && !u.EndsWith("-"))
                    .WithMessage("Username cannot start or end with dot/hyphen/underscore")
                .Must(u => !u.Contains("..") && !u.Contains("__") && !u.Contains("--"))
                    .WithMessage("Username cannot contain consecutive separators (.., __, --)");

            // ===== FullName Validation =====
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Full name is required")
                .Length(1, 250).WithMessage("Full name must be between 1 and 250 characters")
                .Matches("^[\\p{L}\\s.-]+$").WithMessage("Full name can only contain letters, spaces, dots and hyphens");

            // ===== Email Validation =====
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format").When(x => !string.IsNullOrWhiteSpace(x.Email))
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters").When(x => !string.IsNullOrWhiteSpace(x.Email));

            // ===== BranchId Validation =====
            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch ID must be greater than 0");
        }
    }
}