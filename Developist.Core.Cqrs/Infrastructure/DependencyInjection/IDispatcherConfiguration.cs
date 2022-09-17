using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public interface IDispatcherConfiguration
    {
        IRegistryConfiguration AddDefaultDispatcher(ServiceLifetime lifetime = ServiceLifetime.Scoped);
    }
}
