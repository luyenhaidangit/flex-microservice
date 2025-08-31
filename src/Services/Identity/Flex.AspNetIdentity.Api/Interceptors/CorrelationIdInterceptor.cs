using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Flex.AspNetIdentity.Api.Interceptors
{
    public sealed class CorrelationIdInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor accessor;
        
        public CorrelationIdInterceptor(IHttpContextAccessor accessor) 
        {
            this.accessor = accessor;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var cid = accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
            var headers = context.Options.Headers ?? new Metadata();
            headers.Add("x-correlation-id", cid);
            
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, context.Options.WithHeaders(headers));
            return base.AsyncUnaryCall(request, newContext, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var cid = accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
            var headers = context.Options.Headers ?? new Metadata();
            headers.Add("x-correlation-id", cid);
            
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, context.Options.WithHeaders(headers));
            return base.BlockingUnaryCall(request, newContext, continuation);
        }
    }
}

