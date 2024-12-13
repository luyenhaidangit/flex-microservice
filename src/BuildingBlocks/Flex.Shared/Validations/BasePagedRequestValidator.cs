using Flex.Shared.SeedWork;
using FluentValidation;
using System.Reflection;

namespace Flex.Shared.Validations
{
    public abstract class BasePagedRequestValidator<T> : AbstractValidator<T> where T : PagingRequestParameters
    {
        protected BasePagedRequestValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThan(0)
                .WithMessage("PageIndex phải là số nguyên dương lớn hơn 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSize phải là số nguyên dương lớn hơn 0.");

            RuleFor(x => x.OrderBy)
                .Must((request, order) => IsValidOrder(order, GetOrderByMappings(request)))
                .WithMessage((request, order) => $"OrderBy chỉ được nhận {string.Join(", ", GetOrderByMappings(request).Keys)}.")
                .Custom((order, context) =>
                {
                    var orderByMappings = GetOrderByMappings(context.InstanceToValidate);
                    if (order != null && orderByMappings.TryGetValue(order.Trim().ToLower(), out var mappedValue))
                    {
                        context.InstanceToValidate.OrderBy = mappedValue;
                    }
                    else if (string.IsNullOrWhiteSpace(order) || orderByMappings.Count == 0)
                    {
                        context.InstanceToValidate.OrderBy = "";
                        context.InstanceToValidate.SortBy = "";
                    }
                });

            RuleFor(x => x.SortBy)
                .Must(order => IsValidSort(order))
                .WithMessage("SortBy chỉ được nhận 'asc' hoặc 'desc'.");
        }

        private bool IsValidSort(string? order)
        {
            if (string.IsNullOrWhiteSpace(order)) return true;
            return order.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                   order.Equals("desc", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsValidOrder(string? order, Dictionary<string, string> orderByMappings)
        {
            if (string.IsNullOrWhiteSpace(order) || orderByMappings.Count == 0) return true;
            return orderByMappings.ContainsKey(order.Trim().ToLower());
        }

        // Helper method to access protected OrderByMappings
        private Dictionary<string, string> GetOrderByMappings(PagingRequestParameters request)
        {
            return request.GetType().GetProperty("OrderByMappings",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy)
                ?.GetValue(request) as Dictionary<string, string> ?? new Dictionary<string, string>();
        }
    }
}
