namespace DI
{
    public interface IServiceFactory
    {
        public object GetService();
    }

    public interface IServiceFactory<TService> : IServiceFactory
    {
        new TService GetService();
    }
}
