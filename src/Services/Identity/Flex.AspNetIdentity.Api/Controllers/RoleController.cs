using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.AspNetIdentity.Api.Models;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-branches-paging")]
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetRolesPagingRequest request)
        {
            var result = await _roleService.GetRolePagedAsync(request);

            return Ok(Result.Success(result));
        }
    }
}
