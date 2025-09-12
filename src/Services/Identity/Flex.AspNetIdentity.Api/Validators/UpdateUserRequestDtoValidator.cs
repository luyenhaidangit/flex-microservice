using FluentValidation;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Validators
{
    public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MaximumLength(256);

            RuleFor(x => x.FullName)
                .MaximumLength(250);

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}


