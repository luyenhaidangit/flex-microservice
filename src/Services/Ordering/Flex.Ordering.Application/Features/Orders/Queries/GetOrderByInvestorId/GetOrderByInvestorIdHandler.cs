using AutoMapper;
using Flex.Ordering.Application.Common.Interfaces;
using Flex.Ordering.Application.Common.Models;
using Flex.Ordering.Domain.Entities;
using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Queries.GetOrderByInvestorId
{
    public class GetOrderByInvestorIdHandler : IRequestHandler<GetOrderByInvestorIdQuery, Result>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrderByInvestorIdHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<Result> Handle(GetOrderByInvestorIdQuery request, CancellationToken cancellationToken)
        {
            var order = new Order();
            var orderDto = _mapper.Map<OrderDto>(order);

            return Result.Success(orderDto);
        }
    }
}
