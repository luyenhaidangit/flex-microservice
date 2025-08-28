using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Configuration;

namespace Flex.Gateway.Yarp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IProxyConfigProvider _proxyConfigProvider;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(IProxyConfigProvider proxyConfigProvider, ILogger<GatewayController> logger)
        {
            _proxyConfigProvider = proxyConfigProvider;
            _logger = logger;
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetStatus()
        {
            var status = new
            {
                Service = "Flex.Gateway.Yarp",
                Version = "1.0.0",
                Status = "Running",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            return Ok(status);
        }

        [HttpGet("routes")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var config = _proxyConfigProvider.GetConfig();
                var routes = config.Routes.Select(route => new
                {
                    RouteId = route.RouteId,
                    ClusterId = route.ClusterId,
                    Match = route.Match,
                    Transforms = route.Transforms
                });

                return Ok(new
                {
                    TotalRoutes = routes.Count(),
                    Routes = routes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving routes configuration");
                return StatusCode(500, new { error = "Failed to retrieve routes configuration" });
            }
        }

        [HttpGet("clusters")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetClusters()
        {
            try
            {
                var config = _proxyConfigProvider.GetConfig();
                var clusters = config.Clusters.Select(cluster => new
                {
                    ClusterId = cluster.ClusterId,
                    LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                    Destinations = cluster.Destinations.Select(dest => new
                    {
                        DestinationId = dest.Key,
                        Address = dest.Value.Address
                    }),
                    HealthCheck = cluster.HealthCheck
                });

                return Ok(new
                {
                    TotalClusters = clusters.Count(),
                    Clusters = clusters
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clusters configuration");
                return StatusCode(500, new { error = "Failed to retrieve clusters configuration" });
            }
        }

        [HttpGet("metrics")]
        [AllowAnonymous]
        public IActionResult GetMetrics()
        {
            var metrics = new
            {
                Timestamp = DateTime.UtcNow,
                Process = new
                {
                    CpuUsage = Environment.ProcessorCount,
                    MemoryUsage = GC.GetTotalMemory(false),
                    ThreadCount = ThreadPool.ThreadCount,
                    ActiveConnections = 0 // Would need custom implementation to track
                },
                System = new
                {
                    Uptime = Environment.TickCount64,
                    WorkingSet = Environment.WorkingSet,
                    Is64BitProcess = Environment.Is64BitProcess
                }
            };

            return Ok(metrics);
        }
    }
}
