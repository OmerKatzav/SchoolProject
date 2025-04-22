namespace DI;

public class SingletonCreator<TService>(IServiceFactory<TService> serviceFactory) : IServiceCreator<TService>
{
    private TService? _instance;
    private readonly Lock _padlock = new();

    public TService GetService()
    {
        if (_instance != null)
            lock (_padlock)
            {
                return _instance;
            }
        lock (_padlock)
        {
            _instance ??= serviceFactory.GetService();
            return _instance;
        }
    }

    object IServiceCreator.GetService() => GetService()!;
}