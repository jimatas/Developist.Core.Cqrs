using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.Reflection;
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
    /// Represents the default implementation of the <see cref="IDynamicDispatcher"/> interface.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class DynamicDispatcher : IDynamicDispatcher
    {
        private readonly IHandlerRegistry _registry;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDispatcher"/> class with the specified handler registry and optional logger.
        /// </summary>
        /// <param name="registry">The handler registry that the dispatcher will use to look up message handlers.</param>
        /// <param name="logger">An optional logger instance that the dispatcher will use for logging.
        /// If not provided, a <see cref="NullLogger"/> instance will be used.</param>
        public DynamicDispatcher(IHandlerRegistry registry, ILogger<DynamicDispatcher> logger = default)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? NullLogger<DynamicDispatcher>.Instance;
        }

        /// <inheritdoc/>
        async Task IDynamicCommandDispatcher.DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = new ReflectedCommandHandler(command.GetType(), _registry);
            var interceptors = new ReflectedCommandInterceptors(command.GetType(), _registry);
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

            HandlerDelegate Pipe(HandlerDelegate next, ReflectedCommandInterceptor interceptor)
            {
                return () => interceptor.InterceptAsync(command, next, cancellationToken);
            }
        }

        /// <inheritdoc/>
        async Task IDynamicEventDispatcher.DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlers = new ReflectedEventHandlers(@event.GetType(), _registry);
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

            Task SafeHandleAsync(ReflectedEventHandler handler)
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
        async Task<TResult> IDynamicQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), _registry);
            var interceptors = new ReflectedQueryInterceptors<TResult>(query.GetType(), _registry);
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

            HandlerDelegate<TResult> Pipe(HandlerDelegate<TResult> next, ReflectedQueryInterceptor<TResult> interceptor)
            {
                return () => interceptor.InterceptAsync(query, next, cancellationToken);
            }
        }
    }
}
