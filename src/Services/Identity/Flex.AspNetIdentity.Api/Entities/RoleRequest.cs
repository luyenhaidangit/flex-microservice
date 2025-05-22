using System.ComponentModel.DataAnnotations.Schema;
using Flex.Shared.SeedWork.Workflow;

namespace Flex.AspNetIdentity.Api.Entities
{
    [Table("ROLE_REQUESTS")]
    public class RoleRequest : RequestBase<long>
    {
    }
}
