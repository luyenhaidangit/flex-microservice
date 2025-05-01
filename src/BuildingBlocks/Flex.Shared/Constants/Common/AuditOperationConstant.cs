namespace Flex.Shared.Constants.Common
{
    public static class AuditOperationConstant
    {
        // Tác động lên thực thể chính (BranchMaster)
        public const string CreateBranch = "CREATE_BRANCH";
        public const string UpdateBranch = "UPDATE_BRANCH";
        public const string DeleteBranch = "DELETE_BRANCH";

        // Tác động lên request (BranchRequestHeader)
        public const string RejectRequest = "REJECT_REQUEST";
        public const string ApproveRequest = "APPROVE_REQUEST";
        public const string CancelRequest = "CANCEL_REQUEST";
    }
}
