using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests.Helpers;

internal static class ServiceProviderHelper
{
    public static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        return services.BuildServiceProvider();
    }
}
