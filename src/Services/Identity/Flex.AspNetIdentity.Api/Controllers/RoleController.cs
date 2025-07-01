using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.AspNetIdentity.Api.Models;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetRolesPagingRequest request)
        {
            var result = await _roleService.GetRolePagedAsync(request);

            return Ok(Result.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(long id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return Ok(Result.Success(result));
        }

        [HttpPost("requests/create")]
        public async Task<IActionResult> CreateRoleRequest([FromBody] CreateRoleDto dto)
        {
            var id = await _roleService.CreateAddRoleRequestAsync(dto);
            return Ok(Result.Success(id));
        }

        [HttpPost("{roleId}/requests/update")]
        public async Task<IActionResult> CreateUpdateRoleRequest(long roleId, [FromBody] UpdateRoleDto dto)
        {
            var id = await _roleService.CreateUpdateRoleRequestAsync(roleId, dto);
            return Ok(Result.Success(id));
        }

        [HttpPost("{roleId}/requests/delete")]
        public async Task<IActionResult> CreateDeleteRoleRequest(long roleId)
        {
            var id = await _roleService.CreateDeleteRoleRequestAsync(roleId);
            return Ok(Result.Success(id));
        }

        [HttpPost("requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveRoleRequest(long requestId, [FromBody] string? comment)
        {
            await _roleService.ApproveRoleRequestAsync(requestId, comment);
            return Ok(Result.Success());
        }

        [HttpPost("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRoleRequest(long requestId, [FromBody] string reason)
        {
            await _roleService.RejectRoleRequestAsync(requestId, reason);
            return Ok(Result.Success());
        }

        [HttpPost("requests/{requestId}/cancel")]
        public async Task<IActionResult> CancelRoleRequest(long requestId)
        {
            await _roleService.CancelRoleRequestAsync(requestId);
            return Ok(Result.Success());
        }
    }
}
