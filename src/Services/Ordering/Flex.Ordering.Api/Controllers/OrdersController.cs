using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.Shared.DTOs.Ordering;
using Flex.Ordering.Application.Features.Orders.Queries.GetOrderByInvestorId;
using Flex.Ordering.Application.Features.Orders.Queries.GetOrderById;
using Flex.Ordering.Application.Features.Orders.Commands.CreateOrder;
using Flex.Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Flex.Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Flex.Ordering.Application.Features.Orders.Commands.DeleteOrderByInvestor;

namespace Flex.Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("get-by-investor")]
        public async Task<IActionResult> GetOrdersByInvestor([FromQuery] long investorId, CancellationToken cancellationToken)
        {
            var query = new GetOrderByInvestorIdQuery(investorId);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetOrdersById([FromQuery] long id, CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdQuery(id);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest model)
        {
            var command = _mapper.Map<CreateOrderCommand>(model);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("update-order")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest model)
        {
            var command = _mapper.Map<UpdateOrderCommand>(model);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("delete-order")]
        public async Task<IActionResult> DeleteOrder([FromBody] long id)
        {
            var command = new DeleteOrderCommand(id);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("delete-order-by-investor")]
        public async Task<IActionResult> DeleteOrderByInvestor([FromBody] long investorId)
        {
            var command = new DeleteOrderByInvestorCommand(investorId);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
