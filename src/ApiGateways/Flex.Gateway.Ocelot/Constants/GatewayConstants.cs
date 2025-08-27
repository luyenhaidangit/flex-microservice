namespace Flex.Gateway.Ocelot.Constants
{
    public class GatewayConstants
    {
        public const string CorsPolicy = "CorsPolicy";

        public class AuthenticationProviderKey
        {
            public const string AdminPortal = "AdminPortalAuth";   // Thêm key riêng cho CMS / Admin
            public const string Internal = "InternalAuth";   // Dùng cho nội bộ ngân hàng - hệ thống xác thực riêng
            public const string InvestorApp = "InvestorAppAuth";   // Dùng cho nhà đầu tư bên ngoài qua ứng dụng mobile/web
            public const string Partner = "PartnerAuth";   // Dùng cho đối tác tích hợp API
            public const string Sso = "SsoAuth";   // Dùng cho SSO hệ thống (ví dụ: Keycloak, Azure AD)
        }
    }
}
