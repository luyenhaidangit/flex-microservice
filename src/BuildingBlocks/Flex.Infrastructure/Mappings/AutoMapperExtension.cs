using System.Reflection;
using AutoMapper;

namespace Flex.Infrastructure.Mappings
{
    public static class AutoMapperExtension
    {
        // Ignore all non existing properties
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>
       (this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);
            foreach (var property in destinationProperties)
                if (sourceType.GetProperty(property.Name, flags) == null)
                    expression.ForMember(property.Name, opt => opt.Ignore());
            return expression;
        }

        // Ignore all null properties
        public static IMappingExpression<TSource, TDestination> IgnoreNullProperties<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in destinationProperties)
            {
                var sourceProperty = sourceType.GetProperty(property.Name);
                if (sourceProperty == null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
                else
                {
                    // Chỉ ánh xạ khi giá trị không null
                    expression.ForMember(property.Name, opt =>
                        opt.Condition((src, dest, srcValue) => srcValue != null));
                }
            }

            return expression;
        }
    }
}
