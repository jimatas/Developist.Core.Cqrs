namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents a builder for configuring CQRS services.
    /// This class is sealed and cannot be inherited.
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
