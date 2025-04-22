using System.Collections.Concurrent;

namespace DI
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly ConcurrentDictionary<Type, IServiceCreator> _serviceCreators = [];

        public ServiceProvider()
        {
            Register(new SingletonCreator<IServiceProvider>(new LambdaServiceFactory<IServiceProvider>(() => this)));
        }

        public void Register<TService>(IServiceCreator<TService> serviceCreator)
        {
            Register(typeof(TService), serviceCreator);
        }

        public void Register(Type serviceType, IServiceCreator serviceCreator)
        {
            _serviceCreators[serviceType] = serviceCreator;
        }

        public TService GetService<TService>()
        {
            if (_serviceCreators.TryGetValue(typeof(TService), out var serviceCreator))
            {
                return ((IServiceCreator<TService>)serviceCreator).GetService();
            }

            throw new InvalidOperationException($"Service of type {nameof(TService)} is not registered.");
        }

        public object GetService(Type serviceType)
        {
            if (_serviceCreators.TryGetValue(serviceType, out var serviceCreator))
            {
                return (serviceCreator).GetService();
            }

            throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
        }

        public IEnumerable<Type> GetRegisteredTypes()
        {
            return _serviceCreators.Keys;
        }
    }
}
