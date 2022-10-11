using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.Reflection;
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
    public sealed class DynamicDispatcher : IDynamicDispatcher
    {
        private readonly IHandlerRegistry registry;
        private readonly ILogger logger;

        public DynamicDispatcher(IHandlerRegistry registry, ILogger<DynamicDispatcher>? logger = null)
        {
            this.registry = ArgumentNullExceptionHelper.ThrowIfNull(() => registry);
            this.logger = logger ?? NullLogger<DynamicDispatcher>.Instance;
        }

        async Task IDynamicCommandDispatcher.DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => command);

            var handler = new ReflectedCommandHandler(command.GetType(), registry);
            var interceptors = new ReflectedCommandInterceptors(command.GetType(), registry);
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

            HandlerDelegate Pipe(HandlerDelegate next, ReflectedCommandInterceptor interceptor)
            {
                return () => interceptor.InterceptAsync(command, next, cancellationToken);
            }
        }

        async Task IDynamicEventDispatcher.DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => @event);

            var handlers = new ReflectedEventHandlers(@event.GetType(), registry);
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

            Task SafeHandleAsync(ReflectedEventHandler handler)
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

        async Task<TResult> IDynamicQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => query);

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), registry);
            var interceptors = new ReflectedQueryInterceptors<TResult>(query.GetType(), registry);
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

            HandlerDelegate<TResult> Pipe(HandlerDelegate<TResult> next, ReflectedQueryInterceptor<TResult> interceptor)
            {
                return () => interceptor.InterceptAsync(query, next, cancellationToken);
            }
        }
    }
}
