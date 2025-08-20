using FluentValidation;
using Flex.System.Api.Models;
using Flex.Shared.Constants.System.Branch;

namespace Flex.System.Api.Validators
{
    public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequestDto>
    {
        public UpdateBranchRequestValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Branch name is required")
                .Length(3, 200).WithMessage("Branch name must be between 3 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.BranchType)
                .Must(bt => bt == BranchTypeConstants.Branch || bt == BranchTypeConstants.TransactionOffice || bt == BranchTypeConstants.HeadOffice)
                .WithMessage("Invalid branch type");
        }
    }
}


