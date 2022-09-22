using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public sealed class DefaultDispatcher : IDispatcher
    {
        private readonly IHandlerRegistry handlerRegistry;
        private readonly IInterceptorRegistry interceptorRegistry;
        private readonly ILogger logger;

        public DefaultDispatcher(IHandlerRegistry handlerRegistry, IInterceptorRegistry interceptorRegistry, ILogger<DefaultDispatcher>? logger = null)
        {
            this.handlerRegistry = ArgumentNullExceptionHelper.ThrowIfNull(() => handlerRegistry);
            this.interceptorRegistry = ArgumentNullExceptionHelper.ThrowIfNull(() => interceptorRegistry);
            this.logger = logger ?? NullLogger<DefaultDispatcher>.Instance;
        }

        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => command);

            var handler = handlerRegistry.GetCommandHandler<TCommand>();
            var interceptors = interceptorRegistry.GetCommandInterceptors<TCommand>();
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

            var handlers = handlerRegistry.GetEventHandlers<TEvent>();
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

        async Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => query);

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), handlerRegistry);
            var interceptors = new ReflectedQueryInterceptors<TResult>(query.GetType(), interceptorRegistry);
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

        private class ReflectedQueryHandler<TResult>
        {
            private readonly object handler;
            private readonly MethodInfo handleMethod;

            public ReflectedQueryHandler(Type queryType, IHandlerRegistry registry)
            {
                handler = registry.GetQueryHandler(queryType, typeof(TResult));
                handleMethod = handler.GetType().GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))!;
            }

            public Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken)
            {
                try
                {
                    return (Task<TResult>)handleMethod.Invoke(handler, new object[] { query, cancellationToken })!;
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException!).Throw();
                    return Task.FromException<TResult>(exception.InnerException);
                }
            }
        }

        private class ReflectedQueryInterceptors<TResult> : IEnumerable<ReflectedQueryInterceptor<TResult>>
        {
            private readonly IEnumerable<object> interceptors;
            private readonly MethodInfo interceptMethod;

            public ReflectedQueryInterceptors(Type queryType, IInterceptorRegistry registry)
            {
                interceptors = registry.GetQueryInterceptors(queryType, typeof(TResult));
                interceptMethod = typeof(IQueryInterceptor<,>)
                    .MakeGenericType(queryType, typeof(TResult))
                    .GetMethod(nameof(IQueryInterceptor<IQuery<TResult>, TResult>.InterceptAsync))!;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ReflectedQueryInterceptor<TResult>> GetEnumerator()
            {
                return interceptors.Select(interceptor => new ReflectedQueryInterceptor<TResult>(interceptor, interceptMethod)).GetEnumerator();
            }
        }

        private class ReflectedQueryInterceptor<TResult>
        {
            private readonly object interceptor;
            private readonly MethodInfo interceptMethod;

            public ReflectedQueryInterceptor(object interceptor, MethodInfo interceptMethod)
            {
                this.interceptor = interceptor;
                this.interceptMethod = interceptMethod;
            }

            public PriorityLevel Priority => ((IPrioritizable)interceptor).Priority;

            public Task<TResult> InterceptAsync(IQuery<TResult> query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
            {
                try
                {
                    return (Task<TResult>)interceptMethod.Invoke(interceptor, new object[] { query, next, cancellationToken })!;
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException!).Throw();
                    return Task.FromException<TResult>(exception.InnerException);
                }
            }
        }
    }
}
