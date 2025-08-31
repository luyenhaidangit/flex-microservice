//using Flex.AspNetIdentity.Api.Services.Interfaces;
//using Flex.AspNetIdentity.Api.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace Flex.AspNetIdentity.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class GrpcTestController : ControllerBase
//    {
//        private readonly ISystemGateway _systemGateway;
//        private readonly GrpcHealthProbe _healthProbe;

//        public GrpcTestController(ISystemGateway systemGateway, GrpcHealthProbe healthProbe)
//        {
//            _systemGateway = systemGateway;
//            _healthProbe = healthProbe;
//        }

//        [HttpGet("branches")]
//        public async Task<IActionResult> GetBranches([FromQuery] string[] codes, CancellationToken ct)
//        {
//            try
//            {
//                var branches = await _systemGateway.BatchGetBranchesAsync(codes, ct);
//                return Ok(branches);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = ex.Message });
//            }
//        }

//        [HttpGet("health")]
//        public async Task<IActionResult> CheckHealth(CancellationToken ct)
//        {
//            try
//            {
//                var isHealthy = await _healthProbe.IsSystemServiceReadyAsync(ct);
//                return Ok(new { 
//                    service = "SystemService", 
//                    healthy = isHealthy,
//                    timestamp = DateTime.UtcNow
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = ex.Message });
//            }
//        }
//    }
//}

