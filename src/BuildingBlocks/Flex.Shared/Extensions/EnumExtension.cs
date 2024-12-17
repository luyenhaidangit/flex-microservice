using Flex.Shared.Attributes;
using System.Reflection;

namespace Flex.Shared.Extensions
{
    public static class EnumExtension
    {
        public static string ToValue(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttributes(typeof(EnumValueAttribute), false)
                .FirstOrDefault() as EnumValueAttribute;

            return attribute?.Value ?? throw new ArgumentException("Invalid Enum Value");
        }

        public static TEnum FromValue<TEnum>(string value) where TEnum : Enum
        {
            var enumType = typeof(TEnum);
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<EnumValueAttribute>();
                if (attribute?.Value == value)
                {
                    return (TEnum)field.GetValue(null);
                }
            }
            throw new ArgumentException($"No matching enum value for {value}");
        }
    }
}
