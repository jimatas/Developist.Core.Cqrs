namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a configurator for configuring CQRS-related services using an <see cref="IServiceCollection"/>.
/// </summary>
public sealed class CqrsConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CqrsConfigurator"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the CQRS-related services are to be added.</param>
    internal CqrsConfigurator(IServiceCollection services) => Services = services;

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> used for configuring CQRS-related services.
    /// </summary>
    public IServiceCollection Services { get; }
}
