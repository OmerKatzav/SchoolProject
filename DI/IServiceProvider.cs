namespace DI;

public interface IServiceProvider
{
    public void Register<TService>(IServiceCreator<TService> serviceCreator);
    public void Register(Type serviceType, IServiceCreator serviceCreator);
    public TService GetService<TService>();
    public object GetService(Type serviceType);
    public IEnumerable<Type> GetRegisteredTypes();
}