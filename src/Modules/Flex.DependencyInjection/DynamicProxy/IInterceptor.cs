namespace Flex.Core.DynamicProxy
{
    public interface IInterceptor
    {
        Task InterceptAsync(IAbpMethodInvocation invocation);
    }
}
