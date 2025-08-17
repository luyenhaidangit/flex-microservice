namespace Flex.AspNetIdentity.Api.Models.Permission
{
    public sealed class PermissionFlagsResult
    {
        public List<PermissionNodeDto> Root { get; init; } = new List<PermissionNodeDto>();
        public int Total { get; init; }
        public int Assignable { get; init; }
        public int Checked { get; init; }
    }
}
