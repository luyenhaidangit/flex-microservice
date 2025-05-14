using Flex.Core.Collections;
using Flex.Core.DynamicProxy;

namespace Flex.Core.DependencyInjection
{
    public interface IOnServiceRegistredContext
    {
        ITypeList<IInterceptor> Interceptors { get; }

        Type ImplementationType { get; }

        object? ServiceKey { get; }
    }
}
