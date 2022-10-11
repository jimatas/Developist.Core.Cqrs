using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.Reflection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public sealed class Dispatcher : DynamicDispatcherBase, IDispatcher
    {
        public Dispatcher(IHandlerRegistry registry, ILogger<Dispatcher>? logger = null)
            : base(registry, logger) { }

        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => command);

            var handler = HandlerRegistry.GetCommandHandler<TCommand>();
            var interceptors = HandlerRegistry.GetCommandInterceptors<TCommand>();
            try
            {
                await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);
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

            var handlers = HandlerRegistry.GetEventHandlers<TEvent>();
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
                    Logger.LogWarning(exception, "Unhandled exception during event dispatch: {ExceptionMessage}", exception.Message);
                    return Task.FromException(exception);
                }
            }
        }

        async Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => query);

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), HandlerRegistry);
            var interceptors = new ReflectedQueryInterceptors<TResult>(query.GetType(), HandlerRegistry);
            try
            {
                var result = await ExecutePipeline().ConfigureAwait(false);
                return (TResult)result!;
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);
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
