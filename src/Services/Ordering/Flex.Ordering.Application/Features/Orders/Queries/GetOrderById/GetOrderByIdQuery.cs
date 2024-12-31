using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<Result>
    {
        public long Id { get; set; }

        public GetOrderByIdQuery(long id)
        {
            Id = id;
        }
    }
}
