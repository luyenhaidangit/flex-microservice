using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Flex.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedConstantValuesAttribute : ValidationAttribute
    {
        private readonly Type _constantsClass;

        public AllowedConstantValuesAttribute(Type constantsClass)
        {
            if (constantsClass == null)
                throw new ArgumentNullException(nameof(constantsClass));

            if (!constantsClass.IsClass)
                throw new ArgumentException("The type must be a class.", nameof(constantsClass));

            _constantsClass = constantsClass;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult($"{validationContext.DisplayName} is required.");

            // Lấy tất cả các constant trong class
            var validValues = _constantsClass
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetValue(null)?.ToString())
                .ToList();

            // Kiểm tra giá trị có hợp lệ không
            if (!validValues.Contains(value.ToString()))
            {
                return new ValidationResult($"The value '{value}' is not valid for {validationContext.DisplayName}. Valid values are: {string.Join(", ", validValues)}.");
            }

            return ValidationResult.Success;
        }
    }
}
