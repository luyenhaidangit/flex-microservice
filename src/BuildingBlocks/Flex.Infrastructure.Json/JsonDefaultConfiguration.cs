using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Flex.Infrastructure.Json
{
    public sealed class JsonOptionsSetup : IConfigureOptions<JsonOptions>
    {
        public void Configure(JsonOptions options)
        {
            var serializerOptions = options.SerializerOptions;

            serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            serializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            // serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            serializerOptions.WriteIndented = true;
        }
    }
}
