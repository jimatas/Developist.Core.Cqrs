using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IDispatcherConfiguration ConfigureCqrs(this IServiceCollection services) => new CqrsConfigurationBuilder(services);
    }
}
