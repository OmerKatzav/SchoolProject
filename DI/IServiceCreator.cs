namespace DI
{
    public interface IServiceCreator
    {
        public object GetService();
    }

    public interface IServiceCreator<TService> : IServiceCreator
    {
        public new TService GetService();
    }
}
