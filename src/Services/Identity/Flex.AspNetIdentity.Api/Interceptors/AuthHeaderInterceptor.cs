using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Flex.AspNetIdentity.Api.Interceptors
{
    public sealed class AuthHeaderInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor accessor;
        
        public AuthHeaderInterceptor(IHttpContextAccessor accessor) 
        {
            this.accessor = accessor;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var headers = new Metadata();
            var token = accessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add("Authorization", token); // "Bearer xxx"
            }
            
            var newOptions = context.Options.WithHeaders(headers);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);
            return base.AsyncUnaryCall(request, newContext, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var headers = new Metadata();
            var token = accessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add("Authorization", token);
            }
            
            var newOptions = context.Options.WithHeaders(headers);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);
            return base.BlockingUnaryCall(request, newContext, continuation);
        }
    }
}

