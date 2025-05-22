namespace Flex.Shared.Authorization
{
    public static class PermissionConstants
    {
        public static class Securities
        {
            public const string View = "securities.view";
            public const string Edit = "securities.edit";
        }

        public static class Orders
        {
            public const string Place = "orders.place";
            public const string Approve = "orders.approve";
        }

        public static IEnumerable<string> All()
        {
            yield return Securities.View;
            yield return Securities.Edit;
            yield return Orders.Place;
            yield return Orders.Approve;
        }
    }

}
