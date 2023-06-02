using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventDispatcher"/> interface.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IHandlerRegistry _registry;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatcher"/> class with the specified handler registry and optional logger.
        /// </summary>
        /// <param name="registry">The handler registry that the dispatcher will use to look up message handlers.</param>
        /// <param name="logger">An optional logger instance that the dispatcher will use for logging.
        /// If not provided, a <see cref="NullLogger"/> instance will be used.</param>
        public EventDispatcher(IHandlerRegistry registry, ILogger<EventDispatcher> logger = default)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? NullLogger<EventDispatcher>.Instance;
        }

        /// <inheritdoc/>
        public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            if ((object)@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlers = _registry.GetEventHandlers<TEvent>();
            var task = Task.WhenAll(handlers.Select(SafeHandleAsync));

            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                throw task.Exception;
            }

            Task SafeHandleAsync(IEventHandler<TEvent> handler)
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
    }
}
