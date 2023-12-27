using Developist.Core.ArgumentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs;

/// <summary>
/// Provides the default implementation of a registry for resolving command, query, and event handlers, as well as interceptors.
/// </summary>
/// <remarks>
/// This implementation makes use of the built-in dependency injection container to resolve the handlers and interceptors.
/// </remarks>
public sealed class DefaultHandlerRegistry : IHandlerRegistry
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHandlerRegistry"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers and interceptors.</param>
    public DefaultHandlerRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = Ensure.Argument.NotNull(serviceProvider);
    }

    /// <inheritdoc/>
    public ICommandHandler<TCommand> GetCommandHandler<TCommand>()
        where TCommand : ICommand
    {
        var handlers = _serviceProvider.GetServices<ICommandHandler<TCommand>>();

        return handlers.Count() == 1
            ? handlers.Single()
            : throw new InvalidOperationException((handlers.Any() ? "More than one" : "No")
                + $" handler found for command '{typeof(TCommand)}'.");
    }

    /// <inheritdoc/>
    public IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>()
        where TCommand : ICommand
    {
        return _serviceProvider.GetServices<ICommandInterceptor<TCommand>>()
            .OrderBy(interceptor => interceptor.GetPriority());
    }

    /// <inheritdoc/>
    public IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        var handlers = _serviceProvider.GetServices<IQueryHandler<TQuery, TResult>>();

        return handlers.Count() == 1
            ? handlers.Single()
            : throw new InvalidOperationException((handlers.Any() ? "More than one" : "No")
                + $" handler found for query '{typeof(TQuery)}' with result type '{typeof(TResult)}'.");
    }

    /// <inheritdoc/>
    public IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        return _serviceProvider.GetServices<IQueryInterceptor<TQuery, TResult>>()
            .OrderBy(interceptor => interceptor.GetPriority());
    }

    /// <inheritdoc/>
    public IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
        where TEvent : IEvent
    {
        return _serviceProvider.GetServices<IEventHandler<TEvent>>();
    }
}
