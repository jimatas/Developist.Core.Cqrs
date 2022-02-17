// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Utilities;

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
    public class Dispatcher : IDispatcher
    {
        private readonly IHandlerRegistry handlerRegistry;
        private readonly ILogger logger;

        public Dispatcher(IHandlerRegistry handlerRegistry) : this(handlerRegistry, NullLogger<Dispatcher>.Instance) { }
        public Dispatcher(IHandlerRegistry handlerRegistry, ILogger<Dispatcher> logger)
        {
            this.handlerRegistry = Ensure.Argument.NotNull(handlerRegistry, nameof(handlerRegistry));
            this.logger = Ensure.Argument.NotNull(logger, nameof(logger));
        }

        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            Ensure.Argument.NotNull(command, nameof(command));

            var handler = handlerRegistry.GetCommandHandler<TCommand>();
            var wrappers = handlerRegistry.GetCommandHandlerWrappers<TCommand>();
            try
            {
                await ExecutePipeline().WithoutCapturingContext();
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task ExecutePipeline()
            {
                HandlerDelegate pipeline = () => handler.HandleAsync(command, cancellationToken);
                foreach (var wrapper in wrappers.OrderBy(wrapper => (wrapper as IPrioritizable)?.Priority ?? Priorities.Normal))
                {
                    pipeline = Pipe(pipeline, wrapper);
                }
                return pipeline();
            }

            HandlerDelegate Pipe(HandlerDelegate next, ICommandHandlerWrapper<TCommand> wrapper) => () => wrapper.HandleAsync(command, next, cancellationToken);
        }

        async Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            Ensure.Argument.NotNull(query, nameof(query));

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), handlerRegistry);
            var wrappers = new ReflectedQueryHandlerWrappers<TResult>(query.GetType(), handlerRegistry);
            try
            {
                return await ExecutePipeline().WithoutCapturingContext();
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task<TResult> ExecutePipeline()
            {
                HandlerDelegate<TResult> pipeline = () => handler.HandleAsync(query, cancellationToken);
                foreach (var wrapper in wrappers.OrderBy(wrapper => ((IPrioritizable)wrapper).Priority))
                {
                    pipeline = Pipe(pipeline, wrapper);
                }
                return pipeline();
            }

            HandlerDelegate<TResult> Pipe(HandlerDelegate<TResult> next, ReflectedQueryHandlerWrapper<TResult> wrapper) => () => wrapper.HandleAsync(query, next, cancellationToken);
        }

        async Task IEventDispatcher.DispatchAsync<TEvent>(TEvent e, CancellationToken cancellationToken)
        {
            Ensure.Argument.NotNull(e, nameof(e));

            var handlers = handlerRegistry.GetEventHandlers<TEvent>();
            var task = Task.WhenAll(handlers.Select(SafeHandleAsync));
            try
            {
                await task.WithoutCapturingContext();
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
                    return handler.HandleAsync(e, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Unhandled exception during event dispatch: {ExceptionMessage}", exception.Message);
                    return Task.FromException(exception);
                }
            }
        }

        private class ReflectedQueryHandler<TResult>
        {
            private readonly object handler;
            private readonly MethodInfo handleMethod;

            public ReflectedQueryHandler(Type queryType, IHandlerRegistry handlerRegistry)
            {
                handler = handlerRegistry.GetQueryHandler(queryType, typeof(TResult));
                handleMethod = handler.GetType().GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync), BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
            }

            public Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken)
            {
                try
                {
                    return (Task<TResult>)handleMethod.Invoke(handler, new object[] { query, cancellationToken });
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                    return Task.FromException<TResult>(exception.InnerException);
                }
            }
        }

        private class ReflectedQueryHandlerWrappers<TResult> : IEnumerable<ReflectedQueryHandlerWrapper<TResult>>
        {
            private readonly IEnumerable<object> wrappers;
            private readonly MethodInfo handleMethod;

            public ReflectedQueryHandlerWrappers(Type queryType, IHandlerRegistry handlerRegistry)
            {
                wrappers = handlerRegistry.GetQueryHandlerWrappers(queryType, typeof(TResult));
                handleMethod = typeof(IQueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult))
                    .GetMethod(nameof(IQueryHandlerWrapper<IQuery<TResult>, TResult>.HandleAsync), BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<ReflectedQueryHandlerWrapper<TResult>> GetEnumerator()
            {
                return wrappers.Select(wrapper => new ReflectedQueryHandlerWrapper<TResult>(wrapper, handleMethod)).GetEnumerator();
            }
        }

        private class ReflectedQueryHandlerWrapper<TResult> : IPrioritizable
        {
            private readonly object wrapper;
            private readonly MethodInfo handleMethod;

            public ReflectedQueryHandlerWrapper(object wrapper, MethodInfo handleMethod)
            {
                this.wrapper = wrapper;
                this.handleMethod = handleMethod;
            }

            sbyte IPrioritizable.Priority => (wrapper as IPrioritizable)?.Priority ?? Priorities.Normal;

            public Task<TResult> HandleAsync(IQuery<TResult> query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
            {
                try
                {
                    return (Task<TResult>)handleMethod.Invoke(wrapper, new object[] { query, next, cancellationToken });
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                    return Task.FromException<TResult>(exception.InnerException);
                }
            }
        }
    }
}
