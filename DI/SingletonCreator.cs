namespace DI
{
    public class SingletonCreator<TService>(IServiceFactory<TService> serviceFactory) : IServiceCreator<TService>
    {
        private TService? _instance;
        private readonly object padlock = new object();

        public TService GetService()
        {
            if (_instance == null)
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = serviceFactory.GetService();
                    }
                }
            }
            return _instance;
        }

        object IServiceCreator.GetService() => GetService()!;
    }
}
