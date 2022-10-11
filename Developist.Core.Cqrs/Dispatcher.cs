using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public sealed class Dispatcher : IDispatcher
    {
        private readonly IHandlerRegistry registry;
        private readonly ILogger logger;

        public Dispatcher(IHandlerRegistry registry, ILogger<Dispatcher>? logger = null)
        {
            this.registry = ArgumentNullExceptionHelper.ThrowIfNull(() => registry);
            this.logger = logger ?? NullLogger<Dispatcher>.Instance;
        }

        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => command);

            var handler = registry.GetCommandHandler<TCommand>();
            var interceptors = registry.GetCommandInterceptors<TCommand>();
            try
            {
                await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task ExecutePipeline()
            {
                HandlerDelegate pipeline = () => handler.HandleAsync(command, cancellationToken);
                foreach (var interceptor in interceptors.OrderBy(interceptor => interceptor.Priority))
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

        async Task IEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => @event);

            var handlers = registry.GetEventHandlers<TEvent>();
            var task = Task.WhenAll(handlers.Select(SafeHandleAsync));
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
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
                    logger.LogWarning(exception, "Unhandled exception during event dispatch: {ExceptionMessage}", exception.Message);
                    return Task.FromException(exception);
                }
            }
        }

        async Task<TResult> IQueryDispatcher.DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => query);

            var handler = registry.GetQueryHandler<TQuery, TResult>();
            var interceptors = registry.GetQueryInterceptors<TQuery, TResult>();
            try
            {
                return await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task<TResult> ExecutePipeline()
            {
                HandlerDelegate<TResult> pipeline = () => handler.HandleAsync(query, cancellationToken);
                foreach (var interceptor in interceptors.OrderBy(interceptor => interceptor.Priority))
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
