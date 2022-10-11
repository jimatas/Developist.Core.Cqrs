using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.Reflection;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public abstract class DynamicDispatcher : IDynamicCommandDispatcher, IDynamicEventDispatcher
    {
        protected DynamicDispatcher(IHandlerRegistry registry, ILogger? logger = null)
        {
            HandlerRegistry = ArgumentNullExceptionHelper.ThrowIfNull(() => registry);
            Logger = logger ?? NullLoggerFactory.Instance.CreateLogger(GetType());
        }

        protected IHandlerRegistry HandlerRegistry { get; }
        protected ILogger Logger { get; }

        async Task IDynamicCommandDispatcher.DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => command);

            var handler = new ReflectedCommandHandler(command.GetType(), HandlerRegistry);
            var interceptors = new ReflectedCommandInterceptors(command.GetType(), HandlerRegistry);
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

            HandlerDelegate Pipe(HandlerDelegate next, ReflectedCommandInterceptor interceptor)
            {
                return () => interceptor.InterceptAsync(command, next, cancellationToken);
            }
        }

        async Task IDynamicEventDispatcher.DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => @event);

            var handlers = new ReflectedEventHandlers(@event.GetType(), HandlerRegistry);
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
                    Logger.LogWarning(exception, "Unhandled exception during event dispatch: {ExceptionMessage}", exception.Message);
                    return Task.FromException(exception);
                }
            }
        }
    }
}
