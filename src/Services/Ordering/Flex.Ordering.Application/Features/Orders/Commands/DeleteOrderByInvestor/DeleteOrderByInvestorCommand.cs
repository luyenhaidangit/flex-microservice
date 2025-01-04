using Flex.Shared.SeedWork;
using MediatR;
namespace Flex.Ordering.Application.Features.Orders.Commands.DeleteOrderByInvestor
{
    public class DeleteOrderByInvestorCommand : IRequest<Result>
    {
        public long InvestorId { get; set; }

        public DeleteOrderByInvestorCommand(long investorId)
        {
            InvestorId = investorId;
        }
    }
}
