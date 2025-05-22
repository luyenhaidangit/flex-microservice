namespace Flex.Shared.Constants
{
    /// <summary>
    /// Trạng thái nhóm người dùng, dùng để kiểm soát đăng nhập, phân quyền và thao tác trong hệ thống.
    /// </summary>
    /// <remarks>
    /// - Active: Nhóm hoạt động bình thường.
    /// - Inactive: Nhóm bị khoá, mất toàn bộ quyền.
    /// - Suspended: Đình chỉ tạm thời, xử lý như Inactive.
    /// - PendingApproval: Chờ duyệt, chưa được sử dụng.
    ///
    /// Trạng thái ảnh hưởng đến login, phân quyền, giao dịch, phê duyệt, job nền và audit.
    /// </remarks>
    public static class StatusRole
    {
        public const string Active = "Active"; 
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string PendingApproval = "PendingApproval";

        public static readonly string[] All = new[]
        {
            Active,
            Inactive,
            Suspended,
            PendingApproval
        };

        public static bool IsValid(string? value) =>
            All.Contains(value, StringComparer.OrdinalIgnoreCase);
    }
}
