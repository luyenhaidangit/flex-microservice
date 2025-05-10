using FluentValidation;
using Flex.AspNetIdentity.Api.Models.Requests;
using Flex.AspNetIdentity.Api.Services.Interfaces;

namespace Flex.AspNetIdentity.Api.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator(IBranchService branchService)
        {
            RuleFor(x => x.BranchId)
                .MustAsync(async (branchId, cancellation) =>
                {
                    return await branchService.ValidateBranchExistsAsync(branchId);
                })
                .WithMessage("Branch does not exist");

            // Add other validation rules
        }
    }
}
