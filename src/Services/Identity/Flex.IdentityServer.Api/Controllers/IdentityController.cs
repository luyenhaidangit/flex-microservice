using Microsoft.AspNetCore.Mvc;

namespace Flex.IdentityServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok(new { message = "API is working fine!" });
        }
    }
}
