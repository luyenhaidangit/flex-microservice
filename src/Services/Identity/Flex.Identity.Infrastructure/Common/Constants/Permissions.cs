namespace Flex.Identity.Infrastructure.Common.Constants
{
    public static class Permissions
    {
        public const string VIEW = "VIEW";
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";

        public static List<string> GetAllCommands()
        {
            return new List<string>
            {
                VIEW,
                CREATE,
                UPDATE,
                DELETE
            };
        }
    }
}
