namespace DI;

public class TransientCreator<TService>(IServiceFactory<TService> serviceFactory) : IServiceCreator<TService>
{
    public TService GetService()
    {
        return serviceFactory.GetService();
    }
    object IServiceCreator.GetService() => GetService()!;
}