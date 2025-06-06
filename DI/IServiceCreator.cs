namespace DI;

public interface IServiceCreator
{
    public object GetService();
}

public interface IServiceCreator<out TService> : IServiceCreator
{
    public new TService GetService();
}