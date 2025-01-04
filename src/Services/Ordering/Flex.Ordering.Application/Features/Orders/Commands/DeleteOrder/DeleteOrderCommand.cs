using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<Result>
    {
        public long Id { get; set; }

        public DeleteOrderCommand(long id)
        {
            Id = id;
        }
    }
}
