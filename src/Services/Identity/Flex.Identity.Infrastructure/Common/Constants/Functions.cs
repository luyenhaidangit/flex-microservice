namespace Flex.Identity.Infrastructure.Common.Constants
{
    public static class Functions
    {
        public const string Role = "ROLE";
        public const string Product = "PRODUCT";

        public static List<string> GetAllFunctions()
        {
            return new List<string>
            {
                Role,
                Product
            };
        }
    }
}
