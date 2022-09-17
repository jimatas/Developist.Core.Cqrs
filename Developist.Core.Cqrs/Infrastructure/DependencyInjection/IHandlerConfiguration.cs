using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public interface IHandlerConfiguration
    {
        IInterceptorConfiguration AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped);
    }
}
