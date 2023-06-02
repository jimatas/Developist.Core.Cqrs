using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface to add CQRS services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds CQRS services to the specified <see cref="IServiceCollection"/> instance using the provided configuration delegate.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
        /// <param name="configureBuilder">A delegate that configures the <see cref="CqrsBuilder"/> instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsBuilder> configureBuilder)
        {
            if (configureBuilder is null)
            {
                throw new ArgumentNullException(nameof(configureBuilder));
            }

            var builder = new CqrsBuilder(services);
            configureBuilder(builder);

            return services;
        }
    }
}
