using FluentValidation;
using Flex.Workflow.Api.Models.Requests;

namespace Flex.Workflow.Api.Validators
{
    public class ApproveRequestDtoValidator : AbstractValidator<ApproveRequestDto>
    {
        public ApproveRequestDtoValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.ApproverId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Comment).MaximumLength(500);
            RuleFor(x => x.EvidenceUrl).MaximumLength(500);
        }
    }

    public class RejectRequestDtoValidator : AbstractValidator<RejectRequestDto>
    {
        public RejectRequestDtoValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.ApproverId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Reason).MaximumLength(500);
            RuleFor(x => x.EvidenceUrl).MaximumLength(500);
        }
    }
}

