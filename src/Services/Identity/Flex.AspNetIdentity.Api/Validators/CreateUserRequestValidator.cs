using FluentValidation;
using Flex.Shared.DTOs.Identity;

namespace Flex.AspNetIdentity.Api.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches("^[A-Za-z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, hyphens and underscores")
                .Must(u => !u.StartsWith(".") && !u.StartsWith("_") && !u.StartsWith("-") && !u.EndsWith(".") && !u.EndsWith("_") && !u.EndsWith("-"))
                    .WithMessage("Username cannot start or end with dot/hyphen/underscore")
                .Must(u => !u.Contains("..") && !u.Contains("__") && !u.Contains("--"))
                    .WithMessage("Username cannot contain consecutive separators (.., __, --)");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");
        }
    }
}


