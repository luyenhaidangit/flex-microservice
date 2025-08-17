namespace Flex.Shared.Cache
{
    public static class CacheKeys
    {
        // Authentication
        public const string JwtBlacklist = "jwt-blacklist";
        public const string ConfigAuthMode = "config:auth-mode";

        // Permission
        public const string PermissionsTree = "permission:tree";
        public static string PermissionTreeByRole(string roleCode) => $"permission:tree:{roleCode}";
    }
}
