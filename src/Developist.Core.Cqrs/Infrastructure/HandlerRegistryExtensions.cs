using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    /// <summary>
    /// Provides extension methods for working with an <see cref="IHandlerRegistry"/>.
    /// </summary>
    public static class HandlerRegistryExtensions
    {
        /// <summary>
        /// Gets the command handler for the specified command type from the registry.
        /// </summary>
        /// <typeparam name="TCommand">The type of command.</typeparam>
        /// <param name="registry">The <see cref="IHandlerRegistry"/> instance.</param>
        /// <returns>The command handler for the specified command type.</returns>
        public static ICommandHandler<TCommand> GetCommandHandler<TCommand>(this IHandlerRegistry registry)
            where TCommand : ICommand
        {
            return (ICommandHandler<TCommand>)registry.GetCommandHandler(typeof(TCommand));
        }

        /// <summary>
        /// Gets the command interceptors for the specified command type from the registry.
        /// </summary>
        /// <typeparam name="TCommand">The type of command.</typeparam>
        /// <param name="registry">The <see cref="IHandlerRegistry"/> instance.</param>
        /// <returns>The command interceptors for the specified command type.</returns>
        public static IEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>(this IHandlerRegistry registry)
            where TCommand : ICommand
        {
            return registry.GetCommandInterceptors(typeof(TCommand)).Cast<ICommandInterceptor<TCommand>>();
        }

        /// <summary>
        /// Gets the event handlers for the specified event type from the registry.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="registry">The <see cref="IHandlerRegistry"/> instance.</param>
        /// <returns>The event handlers for the specified event type.</returns>
        public static IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>(this IHandlerRegistry registry)
            where TEvent : IEvent
        {
            return registry.GetEventHandlers(typeof(TEvent)).Cast<IEventHandler<TEvent>>();
        }

        /// <summary>
        /// Gets the query handler for the specified query and result types from the registry.
        /// </summary>
        /// <typeparam name="TQuery">The type of query.</typeparam>
        /// <typeparam name="TResult">The type of query result.</typeparam>
        /// <param name="registry">The <see cref="IHandlerRegistry"/> instance.</param>
        /// <returns>The query handler for the specified query and result types.</returns>
        public static IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>(this IHandlerRegistry registry)
            where TQuery : IQuery<TResult>
        {
            return (IQueryHandler<TQuery, TResult>)registry.GetQueryHandler(typeof(TQuery), typeof(TResult));
        }

        /// <summary>
        /// Gets the query interceptors for the specified query and result types from the registry.
        /// </summary>
        /// <typeparam name="TQuery">The type of query.</typeparam>
        /// <typeparam name="TResult">The type of query result.</typeparam>
        /// <param name="registry">The <see cref="IHandlerRegistry"/> instance.</param>
        /// <returns>The query interceptors for the specified query and result types.</returns>
        public static IEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>(this IHandlerRegistry registry)
            where TQuery : IQuery<TResult>
        {
            return registry.GetQueryInterceptors(typeof(TQuery), typeof(TResult)).Cast<IQueryInterceptor<TQuery, TResult>>();
        }
    }
}
