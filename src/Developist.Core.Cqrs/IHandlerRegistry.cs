namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a registry responsible for resolving command, query, and event handlers, as well as interceptors for commands and queries.
/// </summary>
public interface IHandlerRegistry
{
    /// <summary>
    /// Gets the command handler for the specified command type.
    /// </summary>
    /// <typeparam name="TCommand">The type of command for which to retrieve the handler.</typeparam>
    /// <returns>The command handler for the specified command type.</returns>
    /// <exception cref="InvalidOperationException"/>
    ICommandHandler<TCommand> GetCommandHandler<TCommand>()
        where TCommand : ICommand;

    /// <summary>
    /// Gets a collection of command interceptors for the specified command type, ordered by priority.
    /// </summary>
    /// <typeparam name="TCommand">The type of command for which to retrieve the interceptors.</typeparam>
    /// <returns>An ordered collection of command interceptors for the specified command type.</returns>
    IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>()
        where TCommand : ICommand;

    /// <summary>
    /// Gets the query handler for the specified query type and result type.
    /// </summary>
    /// <typeparam name="TQuery">The type of query for which to retrieve the handler.</typeparam>
    /// <typeparam name="TResult">The type of result expected from the query.</typeparam>
    /// <returns>The query handler for the specified query type and result type.</returns>
    /// <exception cref="InvalidOperationException"/>
    IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
        where TQuery : IQuery<TResult>;

    /// <summary>
    /// Gets a collection of query interceptors for the specified query type and result type, ordered by priority.
    /// </summary>
    /// <typeparam name="TQuery">The type of query for which to retrieve the interceptors.</typeparam>
    /// <typeparam name="TResult">The type of result expected from the query.</typeparam>
    /// <returns>An ordered collection of query interceptors for the specified query type and result type.</returns>
    IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
        where TQuery : IQuery<TResult>;

    /// <summary>
    /// Gets a collection of event handlers for the specified type of event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to retrieve handlers for.</typeparam>
    /// <returns>An enumerable collection of event handlers.</returns>
    IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
        where TEvent : IEvent;
}
