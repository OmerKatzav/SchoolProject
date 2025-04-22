using Castle.DynamicProxy;
using DI;
using System.Reflection;

namespace RPC
{
    public class RpcFactory<TService>(DI.IServiceProvider serviceProvider) : IServiceFactory<TService> where TService : class
    {
        public TService GetService()
        {
            if (!typeof(TService).IsInterface) throw new ArgumentException($"Type {typeof(TService)} is not an interface.");
            if (!typeof(TService).IsDefined(typeof(RpcServiceAttribute))) throw new ArgumentException($"Type {typeof(TService)} is not a RPC service.");
            var adapter =
                new AsyncDeterminationInterceptor(new RpcInterceptor(serviceProvider.GetService<INetworkService>()));
            var generator = serviceProvider.GetService<IProxyGenerator>();
            return generator.CreateInterfaceProxyWithoutTarget<TService>(adapter);
        }

        object IServiceFactory.GetService() => GetService();
    }
}
