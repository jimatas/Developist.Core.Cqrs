using Microsoft.Extensions.DependencyInjection;
using System;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for configuring CQRS services using the <see cref="CqrsBuilder"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds CQRS services to the specified <see cref="IServiceCollection"/> instance using the provided configuration delegate.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
        /// <param name="setupAction">A delegate that configures the <see cref="CqrsBuilder"/> instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsBuilder> setupAction)
        {
            if (setupAction is null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var builder = new CqrsBuilder(services);
            setupAction(builder);
            return services;
        }
    }
}
