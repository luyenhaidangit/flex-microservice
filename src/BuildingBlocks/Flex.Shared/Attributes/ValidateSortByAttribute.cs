using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.Attributes
{
    public class ValidateSortByAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sortByValue = value as string;

            // Retrieve the OrderBy property from the current object context
            var orderByProperty = validationContext.ObjectType.GetProperty("OrderBy");
            if (orderByProperty == null)
            {
                return new ValidationResult("OrderBy property not found.");
            }

            var orderByValue = orderByProperty.GetValue(validationContext.ObjectInstance) as string;

            if (!string.IsNullOrEmpty(sortByValue))
            {
                // Ensure OrderBy has a valid value when SortBy is set
                if (string.IsNullOrEmpty(orderByValue))
                {
                    return new ValidationResult("SortBy cannot be set without a valid OrderBy value.");
                }

                sortByValue = sortByValue.Trim().ToUpper();

                // Validate that SortBy is either 'ASC' or 'DESC'
                if (sortByValue != "ASC" && sortByValue != "DESC")
                {
                    return new ValidationResult("SortBy must be either 'ASC' or 'DESC'.");
                }

                // Assign the normalized SortBy value back to the property
                var sortByProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
                if (sortByProperty != null)
                {
                    sortByProperty.SetValue(validationContext.ObjectInstance, sortByValue);
                }
            }

            return ValidationResult.Success;
        }
    }
}
