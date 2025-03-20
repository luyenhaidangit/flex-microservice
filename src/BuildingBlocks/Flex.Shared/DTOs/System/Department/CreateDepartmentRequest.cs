using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Department
{
    public class CreateDepartmentRequest
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Maximum length for Config Key is 255 characters.")]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [MaxLength(500, ErrorMessage = "Maximum length for Description is 500 characters.")]
        public string? Description { get; set; }
    }
}
