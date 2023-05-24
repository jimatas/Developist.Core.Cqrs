using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    /// <summary>
    /// Defines the contract for a registry of handlers and interceptors for commands, events, and queries.
    /// </summary>
    public interface IHandlerRegistry
    {
        /// <summary>
        /// Gets the command handler for the specified command type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>The command handler for the specified command type.</returns>
        ICommandHandler<TCommand> GetCommandHandler<TCommand>()
            where TCommand : ICommand;

        /// <summary>
        /// Gets the command interceptors for the specified command type.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>The command interceptors for the specified command type, in the order they should be executed.</returns>
        IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>()
            where TCommand : ICommand;

        /// <summary>
        /// Gets the event handlers for the specified event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>The event handlers for the specified event type.</returns>
        IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Gets the query handler for the specified query type and result type.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the query result.</typeparam>
        /// <returns>The query handler for the specified query type and result type.</returns>
        IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
            where TQuery : IQuery<TResult>;

        /// <summary>
        /// Gets the query interceptors for the specified query type and result type.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the query result.</typeparam>
        /// <returns>The query interceptors for the specified query type and result type, in the order they should be executed.</returns>
        IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
            where TQuery : IQuery<TResult>;
    }
}
