namespace DI;

public class LambdaServiceFactory<TService>(Func<TService> serviceLambda) : IServiceFactory<TService>
{
    public TService GetService() => serviceLambda();
    object IServiceFactory.GetService() => GetService()!;
}