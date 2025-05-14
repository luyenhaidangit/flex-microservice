namespace Flex.Core.DependencyInjection
{
    public interface IExposedKeyedServiceTypesProvider
    {
        ServiceIdentifier[] GetExposedServiceTypes(Type targetType);
    }
}
