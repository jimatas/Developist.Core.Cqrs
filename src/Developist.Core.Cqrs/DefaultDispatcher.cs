using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Developist.Core.Cqrs;

/// <summary>
/// Provides the default implementation of a CQRS dispatcher that orchestrates the execution of commands, queries, and events.
/// </summary>
public sealed class DefaultDispatcher : IDispatcher
{
    private readonly ConcurrentDictionary<(Type, Type), Delegate> _delegates = new();
    private readonly IHandlerRegistry _registry;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDispatcher"/> class.
    /// </summary>
    /// <param name="registry">The handler registry for resolving handlers and interceptors.</param>
    /// <param name="logger">Optional logger for logging dispatch operations.</param>
    public DefaultDispatcher(IHandlerRegistry registry, ILogger<DefaultDispatcher>? logger = default)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _logger = logger ?? NullLogger<DefaultDispatcher>.Instance;
    }

    /// <inheritdoc/>
    async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var handler = _registry.GetCommandHandler<TCommand>();
        var interceptors = _registry.GetCommandInterceptors<TCommand>();

        try
        {
            await ExecutePipeline(command, handler, interceptors, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);

            throw;
        }
    }

    private static Task ExecutePipeline<TCommand>(
        TCommand command,
        ICommandHandler<TCommand> handler,
        IOrderedEnumerable<ICommandInterceptor<TCommand>> interceptors,
        CancellationToken cancellationToken) where TCommand : ICommand
    {
        CommandHandlerDelegate<TCommand> pipeline = handler.HandleAsync;
        foreach (var interceptor in interceptors)
        {
            pipeline = Pipe(pipeline, interceptor);
        }

        return pipeline.Invoke(command, cancellationToken);
    }

    private static CommandHandlerDelegate<TCommand> Pipe<TCommand>(
        CommandHandlerDelegate<TCommand> next,
        ICommandInterceptor<TCommand> interceptor) where TCommand : ICommand
    {
        return (command, cancellationToken) => interceptor.InterceptAsync(command, next, cancellationToken);
    }

    /// <inheritdoc/>
    Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var queryType = query.GetType();
        var resultType = typeof(TResult);

        var @delegate = (QueryDispatcherDelegate<TResult>)_delegates
            .GetOrAdd((queryType, resultType), _ => CreateDispatcherDelegate<TResult>(queryType, resultType));

        return @delegate.Invoke(query, cancellationToken);
    }

    private Delegate CreateDispatcherDelegate<TResult>(Type queryType, Type resultType)
    {
        var dispatchMethod = typeof(DefaultDispatcher)
            .GetMethods()
            .Single(method => method.Name == nameof(DispatchAsync) && method.GetGenericArguments().Length == 2)
            .MakeGenericMethod(queryType, resultType);

        return Delegate.CreateDelegate(typeof(QueryDispatcherDelegate<TResult>), this, dispatchMethod);
    }

    /// <summary>
    /// Dispatches a query asynchronously to its corresponding handler and returns the result.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to be dispatched.</typeparam>
    /// <typeparam name="TResult">The type of result expected from the query.</typeparam>
    /// <param name="query">The query to be dispatched.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result is the result of the query execution.</returns>
    public async Task<TResult> DispatchAsync<TQuery, TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var handler = _registry.GetQueryHandler<TQuery, TResult>();
        var interceptors = _registry.GetQueryInterceptors<TQuery, TResult>();

        try
        {
            return await ExecutePipeline(query, handler, interceptors, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);

            throw;
        }
    }

    private static Task<TResult> ExecutePipeline<TQuery, TResult>(
        IQuery<TResult> query,
        IQueryHandler<TQuery, TResult> handler,
        IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> interceptors,
        CancellationToken cancellationToken) where TQuery : IQuery<TResult>
    {
        QueryHandlerDelegate<TQuery, TResult> pipeline = handler.HandleAsync;
        foreach (var interceptor in interceptors)
        {
            pipeline = Pipe(pipeline, interceptor);
        }

        return pipeline.Invoke((TQuery)query, cancellationToken);
    }

    private static QueryHandlerDelegate<TQuery, TResult> Pipe<TQuery, TResult>(
        QueryHandlerDelegate<TQuery, TResult> next,
        IQueryInterceptor<TQuery, TResult> interceptor) where TQuery : IQuery<TResult>
    {
        return (query, cancellationToken) => interceptor.InterceptAsync(query, next, cancellationToken);
    }

    /// <inheritdoc/>
    async Task IEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
    {
        if (@event is null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        var handlers = _registry.GetEventHandlers<TEvent>();
        var handleTask = Task.WhenAll(handlers.Select(handler => HandleWithLoggingAsync(@event, handler, cancellationToken)));

        try
        {
            await handleTask.ConfigureAwait(false);
        }
        catch
        {
            throw handleTask.Exception;
        }
    }

    private Task HandleWithLoggingAsync<TEvent>(TEvent @event, IEventHandler<TEvent> handler, CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        try
        {
            return handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Unhandled exception during event dispatch: {ExceptionMessage}", exception.Message);

            return Task.FromException(exception);
        }
    }
}
