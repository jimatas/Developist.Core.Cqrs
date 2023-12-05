namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring CQRS-related services using an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CQRS-related services to the specified <see cref="IServiceCollection"/> and configures them using the provided configurator delegate.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which CQRS services will be added.</param>
    /// <param name="configure">A delegate that configures CQRS services using a <see cref="CqrsConfigurator"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> with CQRS-related services added.</returns>
    public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsConfigurator> configure)
    {
        var configurator = new CqrsConfigurator(services);
        configure(configurator);

        return services;
    }
}
