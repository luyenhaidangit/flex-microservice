//using Grpc.Core;
//using Grpc.Core.Interceptors;
//using Microsoft.Extensions.Logging;

//namespace Flex.AspNetIdentity.Api.Interceptors
//{
//    public sealed class ClientLoggingInterceptor : Interceptor
//    {
//        private readonly ILogger<ClientLoggingInterceptor> logger;

//        public ClientLoggingInterceptor(ILogger<ClientLoggingInterceptor> logger)
//        {
//            this.logger = logger;
//        }

//        public override async Task<TResponse> UnaryCall<TRequest, TResponse>(
//            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, UnaryCallContinuation<TRequest, TResponse> continuation)
//        {
//            var sw = System.Diagnostics.Stopwatch.StartNew();
//            try
//            {
//                var resp = await continuation(request, context);
//                logger.LogInformation("gRPC {Method} OK in {Elapsed} ms", context.Method.FullName, sw.ElapsedMilliseconds);
//                return resp;
//            }
//            catch (RpcException ex)
//            {
//                logger.LogWarning(ex, "gRPC {Method} failed: {Status} ({Detail}) after {Elapsed} ms",
//                    context.Method.FullName, ex.StatusCode, ex.Status.Detail, sw.ElapsedMilliseconds);
//                throw;
//            }
//        }

//        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
//            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
//        {
//            var sw = System.Diagnostics.Stopwatch.StartNew();
//            var call = continuation(request, context);

//            Wrap the response to add logging
//            var responseAsync = call.ResponseAsync.ContinueWith(t =>
//            {
//                if (t.IsCompletedSuccessfully)
//                {
//                    logger.LogInformation("gRPC {Method} OK in {Elapsed} ms", context.Method.FullName, sw.ElapsedMilliseconds);
//                }
//                else if (t.IsFaulted && t.Exception?.InnerException is RpcException rpcEx)
//                {
//                    logger.LogWarning(rpcEx, "gRPC {Method} failed: {Status} ({Detail}) after {Elapsed} ms",
//                        context.Method.FullName, rpcEx.StatusCode, rpcEx.Status.Detail, sw.ElapsedMilliseconds);
//                }
//                return t.Result;
//            });

//            return new AsyncUnaryCall<TResponse>(
//                responseAsync,
//                call.ResponseHeadersAsync,
//                call.GetStatus,
//                call.GetTrailers,
//                call.Dispose);
//        }
//    }
//}

