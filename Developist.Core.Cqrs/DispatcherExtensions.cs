using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs
{
    public static class DispatcherExtensions
    {
        public static DispatcherDelegate<TCommand> CreateDelegate<TCommand>(this ICommandDispatcher commandDispatcher)
            where TCommand : ICommand
        {
            return (command, cancellationToken) => commandDispatcher.DispatchAsync(command, cancellationToken);
        }

        public static DispatcherDelegate<TEvent> CreateDelegate<TEvent>(this IEventDispatcher eventDispatcher)
            where TEvent : IEvent
        {
            return (@event, cancellationToken) => eventDispatcher.DispatchAsync(@event, cancellationToken);
        }

        public static DispatcherDelegate<TQuery, TResult> CreateDelegate<TQuery, TResult>(this IQueryDispatcher queryDispatcher)
            where TQuery : IQuery<TResult>
        {
            return (query, cancellationToken) => queryDispatcher.DispatchAsync<TQuery, TResult>(query, cancellationToken);
        }
    }
}
