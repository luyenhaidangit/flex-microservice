using FluentValidation;
using Flex.Workflow.Api.Models.Requests;

namespace Flex.Workflow.Api.Validators
{
    public class CreateApprovalRequestDtoValidator : AbstractValidator<CreateApprovalRequestDto>
    {
        public CreateApprovalRequestDtoValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Domain).NotEmpty().MaximumLength(50);
            RuleFor(x => x.WorkflowCode).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Action).NotEmpty().MaximumLength(20);
            RuleFor(x => x.MakerId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.BusinessId).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.BusinessId));
            RuleFor(x => x.CorrelationId).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.CorrelationId));
        }
    }
}

