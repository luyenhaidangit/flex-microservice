using System.ComponentModel.DataAnnotations;

namespace Flex.Infrastructure.Attributes
{
    public class DependentOrderByAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sortByValue = value as string;

            var orderByProperty = validationContext.ObjectType.GetProperty("OrderBy");
            if (orderByProperty == null)
            {
                return new ValidationResult("OrderBy property not found.");
            }

            var orderByValue = orderByProperty.GetValue(validationContext.ObjectInstance) as string;

            if (!string.IsNullOrEmpty(sortByValue) && string.IsNullOrEmpty(orderByValue))
            {
                return new ValidationResult("SortBy cannot be set without a valid OrderBy value.");
            }

            return ValidationResult.Success;
        }
    }
}
