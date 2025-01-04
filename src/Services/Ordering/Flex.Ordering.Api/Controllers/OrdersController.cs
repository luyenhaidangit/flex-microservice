using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.Ordering.Application.Features.Orders.Queries.GetOrderByInvestorId;
using Flex.Ordering.Application.Features.Orders.Queries.GetOrderById;

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
    }
}
