using DI;
using System.Reflection;

namespace RPC;

public static class ServiceProviderExtensions
{
    public static void RegisterRpcSingleton<TService>(this DI.IServiceProvider serviceProvider) where TService : class
    {
        if (!typeof(TService).IsInterface) throw new ArgumentException($"Type {typeof(TService)} is not an interface.");
        if (!typeof(TService).IsDefined(typeof(RpcServiceAttribute))) throw new ArgumentException($"Type {typeof(TService)} is not a RPC service.");
        serviceProvider.Register(new SingletonCreator<TService>(new RpcFactory<TService>(serviceProvider)));
    }

    public static void RegisterRpcTransient<TService>(this DI.IServiceProvider serviceProvider) where TService : class
    {
        if (!typeof(TService).IsInterface) throw new ArgumentException($"Type {typeof(TService)} is not an interface.");
        if (!typeof(TService).IsDefined(typeof(RpcServiceAttribute))) throw new ArgumentException($"Type {typeof(TService)} is not a RPC service.");
        serviceProvider.Register(new TransientCreator<TService>(new RpcFactory<TService>(serviceProvider)));
    }
}