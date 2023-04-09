using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
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
        private readonly IHandlerRegistry _registry;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher"/> class with the specified handler registry and optional logger.
        /// </summary>
        /// <param name="registry">The handler registry that the dispatcher will use to look up message handlers.</param>
        /// <param name="logger">An optional logger instance that the dispatcher will use for logging.
        /// If not provided, a <see cref="NullLogger"/> instance will be used.</param>
        public Dispatcher(IHandlerRegistry registry, ILogger<Dispatcher> logger = default)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? NullLogger<Dispatcher>.Instance;
        }

        /// <inheritdoc/>
        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            if ((object)command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _registry.GetCommandHandler<TCommand>();
            var interceptors = _registry.GetCommandInterceptors<TCommand>();
            try
            {
                await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task ExecutePipeline()
            {
                HandlerDelegate pipeline = () => handler.HandleAsync(command, cancellationToken);
                foreach (var interceptor in interceptors)
                {
                    pipeline = Pipe(pipeline, interceptor);
                }
                return pipeline();
            }

            HandlerDelegate Pipe(HandlerDelegate next, ICommandInterceptor<TCommand> interceptor)
            {
                return () => interceptor.InterceptAsync(command, next, cancellationToken);
            }
        }

        /// <inheritdoc/>
        async Task IEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
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
                if (task.Exception is AggregateException exception)
                {
                    throw exception;
                }
                throw;
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

        /// <inheritdoc/>
        async Task<TResult> IQueryDispatcher.DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        {
            if ((object)query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = _registry.GetQueryHandler<TQuery, TResult>();
            var interceptors = _registry.GetQueryInterceptors<TQuery, TResult>();
            try
            {
                return await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task<TResult> ExecutePipeline()
            {
                HandlerDelegate<TResult> pipeline = () => handler.HandleAsync(query, cancellationToken);
                foreach (var interceptor in interceptors)
                {
                    pipeline = Pipe(pipeline, interceptor);
                }
                return pipeline();
            }

            HandlerDelegate<TResult> Pipe(HandlerDelegate<TResult> next, IQueryInterceptor<TQuery, TResult> interceptor)
            {
                return () => interceptor.InterceptAsync(query, next, cancellationToken);
            }
        }
    }
}
