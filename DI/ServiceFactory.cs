using System.Linq.Expressions;

namespace DI
{
    public class ServiceFactory<TService, TImplementation>(IServiceProvider serviceProvider) : LambdaServiceFactory<TService>(GetServiceFunc(serviceProvider)) where TImplementation : TService
    {
        private static Func<TService> GetServiceFunc(IServiceProvider serviceProvider)
        {
            var constructor = typeof(TImplementation).GetConstructors().First();
            var getService = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService)) ?? throw new MissingMethodException(nameof(IServiceProvider), nameof(IServiceProvider.GetService));
            var arguments = constructor.GetParameters().Select(p => Expression.Call(Expression.Constant(serviceProvider), getService, Expression.Constant(p.ParameterType)));
            return Expression.Lambda<Func<TService>>(Expression.New(constructor, arguments)).Compile();
        }
    }
}
