using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Flex.Shared.Attributes
{
    public class ValidateOrderByAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Cast the OrderBy value to string
            var orderByValue = value as string;

            // Allow OrderBy to be empty (no validation needed)
            if (string.IsNullOrEmpty(orderByValue))
            {
                return ValidationResult.Success;
            }

            // Retrieve the OrderByMappings property from the current object context
            var orderByMappingsProperty = validationContext.ObjectType.GetProperty("OrderByMappings",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (orderByMappingsProperty == null)
            {
                return new ValidationResult("OrderByMappings property not found.");
            }

            // Get the dictionary of valid mappings
            var orderByMappings = orderByMappingsProperty.GetValue(validationContext.ObjectInstance) as Dictionary<string, string>;
            if (orderByMappings == null || orderByMappings.Count == 0)
            {
                return new ValidationResult("OrderByMappings is not properly configured.");
            }

            // Normalize the value (trim spaces and convert to uppercase)
            orderByValue = orderByValue.Trim().ToUpper();

            // Check if the value exists in OrderByMappings keys
            if (!orderByMappings.ContainsKey(orderByValue))
            {
                return new ValidationResult($"OrderBy value '{orderByValue}' is invalid. Allowed values: {string.Join(", ", orderByMappings.Keys)}.");
            }

            // Assign the normalized value back to the property
            var orderByProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
            if (orderByProperty != null)
            {
                orderByProperty.SetValue(validationContext.ObjectInstance, orderByValue);
            }

            // Validation succeeds
            return ValidationResult.Success;
        }
    }
}
