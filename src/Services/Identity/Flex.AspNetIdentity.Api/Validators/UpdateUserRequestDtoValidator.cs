using FluentValidation;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Validators
{
    public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserRequestDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(250);

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}


