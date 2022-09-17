using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public interface IInterceptorConfiguration : IHandlerConfiguration
    {
        IInterceptorConfiguration AddInterceptorsFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped);
    }
}
