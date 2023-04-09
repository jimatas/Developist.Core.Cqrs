using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Represents a builder for configuring CQRS services.
    /// </summary>
    public sealed class CqrsBuilder
    {
        internal CqrsBuilder(IServiceCollection services) => Services = services;

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> instance used by this builder.
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
