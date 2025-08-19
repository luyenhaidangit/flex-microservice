using FluentValidation;
using Flex.System.Api.Models;
using Flex.Shared.Constants.System.Branch;

namespace Flex.System.Api.Validators
{
    public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequestDto>
    {
        public CreateBranchRequestValidator()
        {
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Branch code is required")
                .Length(3, 50).WithMessage("Branch code must be between 3 and 50 characters")
                .Matches("^[A-Za-z0-9-_]+$").WithMessage("Branch code can only contain letters, numbers, hyphens and underscores")
                .Must(code => !code.StartsWith("-") && !code.StartsWith("_") && !code.EndsWith("-") && !code.EndsWith("_"))
                    .WithMessage("Branch code cannot start or end with hyphen/underscore")
                .Must(code => !code.Contains("--") && !code.Contains("__"))
                    .WithMessage("Branch code cannot contain consecutive hyphens/underscores");

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
