﻿using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Ordering.Domain.Entities
{
    public class Order : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string UserName { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(250)")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ShippingAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string InvoiceAddress { get; set; }
    }
}