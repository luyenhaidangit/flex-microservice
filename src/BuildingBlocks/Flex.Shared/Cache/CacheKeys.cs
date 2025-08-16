namespace Flex.Shared.Cache
{
    public class CacheKeys
    {
        // Permission
        public const string PermissionCatalog = "permission:catalog";
        public static string PermissionTreeByRole(string roleCode) => $"permission:tree:{roleCode}";
    }
}
