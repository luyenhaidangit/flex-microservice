using Flex.Core.DependencyInjection;

namespace Flex.Data
{
    [Dependency(ReplaceServices = true)]
    public class ConnectionStringCheckResult
    {
        public bool Connected { get; set; }

        public bool DatabaseExists { get; set; }
    }
}
