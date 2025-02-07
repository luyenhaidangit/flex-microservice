using System.Text.Json.Serialization;
using System.Text.Json;

namespace Flex.Shared.Options
{
    public static class JsonConfig
    {
        public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }
}
