using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace Flex.Swashbuckle
{
    public class LowerCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.ToDictionary(
                path => PreserveDynamicParameters(path.Key),
                path => path.Value
            );

            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var pathItem in paths)
            {
                swaggerDoc.Paths.Add(pathItem.Key, pathItem.Value);
            }
        }

        private string PreserveDynamicParameters(string path)
        {
            var regex = new Regex(@"\{[^/}]+}");

            return regex.Replace(path, match => match.Value)
                        .Split('/')
                        .Select(segment => regex.IsMatch(segment) ? segment : segment.ToLowerInvariant())
                        .Aggregate((current, next) => $"{current}/{next}");
        }
    }
}
