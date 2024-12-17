namespace Flex.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumValueAttribute : Attribute
    {
        public string Value { get; }

        public EnumValueAttribute(string value)
        {
            Value = value;
        }
    }
}
