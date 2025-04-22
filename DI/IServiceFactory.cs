namespace DI
{
    public interface IServiceFactory
    {
        public object GetService();
    }

    public interface IServiceFactory<out TService> : IServiceFactory
    {
        new TService GetService();
    }
}
