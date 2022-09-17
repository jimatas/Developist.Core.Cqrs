using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IDispatcherConfiguration AddCqrs(this IServiceCollection services) => new CqrsBuilder(services);
    }
}
