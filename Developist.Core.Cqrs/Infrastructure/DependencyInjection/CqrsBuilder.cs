using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public sealed class CqrsBuilder
    {
        internal CqrsBuilder(IServiceCollection services) => Services = services;

        public IServiceCollection Services { get; }
    }
}
