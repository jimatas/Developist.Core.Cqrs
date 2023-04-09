using System;
using System.Collections.Generic;

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
        /// <param name="commandType">The type of the command.</param>
        /// <returns>The command handler.</returns>
        /// <exception cref="InvalidOperationException">When no handler is registered for the specified command type, or when multiple handlers are registered.</exception>
        object GetCommandHandler(Type commandType);

        /// <summary>
        /// Gets the command interceptors for the specified command type.
        /// </summary>
        /// <param name="commandType">The type of the command.</param>
        /// <returns>An enumerable collection of command interceptors, in the order they should be executed.</returns>
        IEnumerable<object> GetCommandInterceptors(Type commandType);

        /// <summary>
        /// Gets the event handlers for the specified event type.
        /// </summary>
        /// <param name="eventType">The type of the event.</param>
        /// <returns>An enumerable collection of event handlers.</returns>
        IEnumerable<object> GetEventHandlers(Type eventType);

        /// <summary>
        /// Gets the query handler for the specified query and result types.
        /// </summary>
        /// <param name="queryType">The type of the query.</param>
        /// <param name="resultType">The type of the query result.</param>
        /// <returns>The query handler.</returns>
        /// <exception cref="InvalidOperationException">When no handler is registered for the specified query and result types, or when multiple handlers are registered.</exception>
        object GetQueryHandler(Type queryType, Type resultType);

        /// <summary>
        /// Gets the query interceptors for the specified query and result types.
        /// </summary>
        /// <param name="queryType">The type of the query.</param>
        /// <param name="resultType">The type of the query result.</param>
        /// <returns>An enumerable collection of query interceptors, in the order they should be executed.</returns>
        IEnumerable<object> GetQueryInterceptors(Type queryType, Type resultType);
    }
}
