using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Queries.GetOrderByInvestorId
{
    public class GetOrderByInvestorIdQuery : IRequest<Result>
    {
        public long InvestorId { get; set; }

        public GetOrderByInvestorIdQuery(long investorId)
        {
            InvestorId = investorId;
        }
    }
}
