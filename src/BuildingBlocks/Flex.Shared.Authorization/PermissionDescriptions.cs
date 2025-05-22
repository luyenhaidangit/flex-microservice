using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Shared.Authorization
{
    public static class PermissionDescriptions
    {
        public static readonly Dictionary<string, string> Map = new()
    {
        { PermissionConstants.Securities.View, "Xem danh sách chứng khoán" },
        { PermissionConstants.Orders.Approve, "Phê duyệt đơn hàng" },
    };
    }
}
