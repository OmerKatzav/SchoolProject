namespace DI;

public static class ServiceProviderExtensions
{
    public static void RegisterSingleton<TService, TImplementation>(this IServiceProvider serviceProvider) where TImplementation : TService where TService : class
    {
        serviceProvider.Register(new SingletonCreator<TService>(new ServiceFactory<TService, TImplementation>(serviceProvider)));
    }

    public static void RegisterSingleton<TService>(this IServiceProvider serviceProvider, Func<TService> factory) where TService : class
    {
        serviceProvider.Register(new SingletonCreator<TService>(new LambdaServiceFactory<TService>(factory)));
    }

    public static void RegisterTransient<TService, TImplementation>(this IServiceProvider serviceProvider) where TImplementation : TService where TService : class
    {
        serviceProvider.Register(new TransientCreator<TService>(new ServiceFactory<TService, TImplementation>(serviceProvider)));
    }

    public static void RegisterTransient<TService>(this IServiceProvider serviceProvider, Func<TService> factory) where TService : class
    {
        serviceProvider.Register(new TransientCreator<TService>(new LambdaServiceFactory<TService>(factory)));
    }
}