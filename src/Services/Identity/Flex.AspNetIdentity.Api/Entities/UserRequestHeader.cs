using System.ComponentModel.DataAnnotations.Schema;
using Flex.Contracts.Domains;

namespace Flex.System.Api.Entities
{
    [Table("USER_REQUEST_HEADER")]
    public class UserRequestHeader : RequestHeaderBase
    {
        // Inherited from RequestHeaderBase:
        // - Action (Create/Update/Delete)
        // - Status (Unauthorised/Approved/Rejected)
        // - RequestedBy, RequestedDate
        // - ApproveBy, ApproveDate
        // - Comments
    }
}
