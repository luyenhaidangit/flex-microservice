﻿using Flex.Ordering.Application.Common.Mappings;
using Flex.Ordering.Domain.Entities;
using Flex.Shared.SeedWork;
using MediatR;

namespace Flex.Ordering.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IMapFrom<Order>, IRequest<Result>
    {
        public long InvestorId { get; set; }

        public decimal TotalPrice { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? InvoiceAddress { get; set; }
    }
}
