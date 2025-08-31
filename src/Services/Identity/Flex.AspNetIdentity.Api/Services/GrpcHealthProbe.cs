//using Grpc.Health.V1;

//namespace Flex.AspNetIdentity.Api.Services
//{
//    public sealed class GrpcHealthProbe
//    {
//        private readonly Health.HealthClient health;
        
//        public GrpcHealthProbe(Health.HealthClient health) 
//        {
//            this.health = health;
//        }

//        public async Task<bool> IsReadyAsync(string serviceName, CancellationToken ct)
//        {
//            try
//            {
//                var resp = await health.CheckAsync(new HealthCheckRequest { Service = serviceName }, cancellationToken: ct);
//                return resp.Status == HealthCheckResponse.Types.ServingStatus.Serving;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<bool> IsSystemServiceReadyAsync(CancellationToken ct)
//        {
//            return await IsReadyAsync("flex.system.grpc.services.BranchService", ct);
//        }
//    }
//}

