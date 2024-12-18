﻿using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public abstract class CreateOrUpdateIssuerDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Maximum length for Issuer Code is 50 characters.")]
        public string Code { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Maximum length for Issuer Name is 250 characters.")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Maximum length for Description is 1000 characters.")]
        public string? Description { get; set; }
    }
}
