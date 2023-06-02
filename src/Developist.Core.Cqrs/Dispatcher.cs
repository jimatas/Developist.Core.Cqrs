using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IDispatcher"/> interface.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class Dispatcher : IDispatcher
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher"/> class with the specified command, event, and query dispatchers.
        /// </summary>
        /// <param name="commandDispatcher">The command dispatcher.</param>
        /// <param name="eventDispatcher">The event dispatcher.</param>
        /// <param name="queryDispatcher">The query dispatcher.</param>
        public Dispatcher(ICommandDispatcher commandDispatcher, IEventDispatcher eventDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        }

        /// <inheritdoc/>
        Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            return _commandDispatcher.DispatchAsync(command, cancellationToken);
        }

        /// <inheritdoc/>
        Task IEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            return _eventDispatcher.DispatchAsync(@event, cancellationToken);
        }

        /// <inheritdoc/>
        Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return _queryDispatcher.DispatchAsync(query, cancellationToken);
        }
    }
}
