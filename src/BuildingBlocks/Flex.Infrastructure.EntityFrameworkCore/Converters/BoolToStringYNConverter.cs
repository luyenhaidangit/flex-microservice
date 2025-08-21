using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flex.Infrastructure.EntityFrameworkCore.Converters
{
    public class BoolToStringYNConverter : ValueConverter<bool, string>
    {
        public BoolToStringYNConverter()
            : base(
                v => v ? "Y" : "N",
                v => v == "Y"
            )
        {
        }
    }
}
