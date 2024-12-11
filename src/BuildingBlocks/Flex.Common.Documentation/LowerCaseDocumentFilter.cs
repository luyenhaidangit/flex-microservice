using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flex.Common.Documentation
{
    public class LowerCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.ToDictionary(
                path => path.Key.ToLowerInvariant(),
                path => path.Value
            );

            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var pathItem in paths)
            {
                swaggerDoc.Paths.Add(pathItem.Key, pathItem.Value);
            }
        }
    }
}
