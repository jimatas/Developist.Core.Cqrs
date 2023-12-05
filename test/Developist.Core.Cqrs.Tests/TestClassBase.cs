using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public abstract class TestClassBase
{
    protected static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);

        EnsureQueueRegistration(services);

        return services.BuildServiceProvider();
    }

    private static void EnsureQueueRegistration(ServiceCollection services)
    {
        if (!services.Any(service => service.ServiceType == typeof(Queue<object>)))
        {
            services.AddScoped<Queue<object>>();
        }
    }
}
