using AutoMapper;
using Flex.Ordering.Application.Common.Interfaces;
using Flex.Ordering.Application.Common.Models;
using Flex.Ordering.Domain.Entities;
using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<Result> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = new Order();
            var orderDto = _mapper.Map<OrderDto>(order);

            return Result.Success(orderDto);
        }
    }
}
