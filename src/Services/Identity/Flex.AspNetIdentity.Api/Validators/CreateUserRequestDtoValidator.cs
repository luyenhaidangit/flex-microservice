using FluentValidation;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Validators
{
    public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50)
                .Matches("^[A-Za-z0-9._-]+$")
                .Must(u => !u.StartsWith('.') && !u.StartsWith('_') && !u.StartsWith('-') && !u.EndsWith('.') && !u.EndsWith('_') && !u.EndsWith('-'))
                .WithMessage("Username cannot start or end with dot/hyphen/underscore")
                .Must(u => !u.Contains("..") && !u.Contains("__") && !u.Contains("--"))
                .WithMessage("Username cannot contain consecutive separators (.., __, --)");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required")
                .MaximumLength(250);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50);
        }
    }
}


