using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    public static class HandlerRegistryExtensions
    {
        public static ICommandHandler<TCommand> GetCommandHandler<TCommand>(this IHandlerRegistry registry)
            where TCommand : ICommand
        {
            return (ICommandHandler<TCommand>)registry.GetCommandHandler(typeof(TCommand));
        }

        public static IEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>(this IHandlerRegistry registry)
            where TCommand : ICommand
        {
            return registry.GetCommandInterceptors(typeof(TCommand)).Cast<ICommandInterceptor<TCommand>>();
        }

        public static IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>(this IHandlerRegistry registry)
            where TEvent : IEvent
        {
            return registry.GetEventHandlers(typeof(TEvent)).Cast<IEventHandler<TEvent>>();
        }

        public static IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>(this IHandlerRegistry registry)
            where TQuery : IQuery<TResult>
        {
            return (IQueryHandler<TQuery, TResult>)registry.GetQueryHandler(typeof(TQuery), typeof(TResult));
        }

        public static IEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>(this IHandlerRegistry registry)
            where TQuery : IQuery<TResult>
        {
            return registry.GetQueryInterceptors(typeof(TQuery), typeof(TResult)).Cast<IQueryInterceptor<TQuery, TResult>>();
        }
    }
}
