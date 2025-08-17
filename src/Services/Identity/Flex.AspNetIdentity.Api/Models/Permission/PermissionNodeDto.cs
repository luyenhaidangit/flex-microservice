namespace Flex.AspNetIdentity.Api.Models.Permission
{
    public sealed class PermissionNodeDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsAssignable { get; set; }  // node lá hay node nhóm
        public bool IsChecked { get; set; }     // đã gán cho role?
        public bool IsIndeterminate { get; set; } // tính ở FE khi có children mixed
        public int IsCrudRule { get; set; }
        public int SortOrder { get; set; }
        public List<PermissionNodeDto> Children { get; set; } = new();
    }
}
