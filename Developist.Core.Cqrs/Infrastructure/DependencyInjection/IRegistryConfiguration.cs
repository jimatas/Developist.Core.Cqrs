using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public interface IRegistryConfiguration
    {
        IHandlerConfiguration AddDefaultRegistry(ServiceLifetime lifetime = ServiceLifetime.Scoped);
    }
}
