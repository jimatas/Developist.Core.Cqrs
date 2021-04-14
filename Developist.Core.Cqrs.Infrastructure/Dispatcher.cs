﻿// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        #region Constructors
        public Dispatcher(IServiceProvider serviceProvider) : this(serviceProvider, logger: null) { }
        public Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? serviceProvider.GetService<ILogger<Dispatcher>>() ?? NullLogger<Dispatcher>.Instance;
        }
        #endregion

        [DebuggerStepThrough]
        async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            var wrappers = serviceProvider.GetServices<ICommandHandlerWrapper<TCommand>>();
            try
            {
                await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Exception thrown during command dispatch: {ExceptionDetailMessage}", exception.DetailMessage(true));
                throw;
            }

            Task ExecutePipeline()
            {
                HandlerDelegate pipeline = () => handler.HandleAsync(command, cancellationToken);
                foreach (var wrapper in wrappers.OrderByDescending(wrapper => wrapper.SortOrder))
                {
                    pipeline = Pipe(pipeline, wrapper);
                }
                return pipeline();
            }

            HandlerDelegate Pipe(HandlerDelegate next, ICommandHandlerWrapper<TCommand> wrapper) => () => wrapper.HandleAsync(command, next, cancellationToken);
        }

        [DebuggerStepThrough]
        async Task<TResult> IQueryDispatcher.DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = new ReflectedQueryHandler<TResult>(query.GetType(), serviceProvider);
            var wrappers = new ReflectedQueryHandlerWrappers<TResult>(query.GetType(), serviceProvider);
            try
            {
                return await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Exception thrown during query dispatch: {ExceptionDetailMessage}", exception.DetailMessage(true));
                throw;
            }

            Task<TResult> ExecutePipeline()
            {
                HandlerDelegate<TResult> pipeline = () => handler.InvokeAsync(query, cancellationToken);
                foreach (object wrapper in wrappers.Cast<ISortable>().OrderByDescending(wrapper => wrapper.SortOrder))
                {
                    pipeline = Pipe(pipeline, wrapper);
                }
                return pipeline();
            }

            HandlerDelegate<TResult> Pipe(object next, object wrapper) => () => wrappers.InvokeAsync(wrapper, query, next, cancellationToken);
        }

        [DebuggerStepThrough]
        async Task IEventDispatcher.DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            ICollection<Exception> exceptions = null;
            var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
            try
            {
                await Task.WhenAll(handlers.Select(HandleAsyncWithException)).ConfigureAwait(false);
            }
            catch
            {
                if (exceptions is not null)
                {
                    throw new AggregateException(exceptions);
                }
            }

            Task HandleAsyncWithException(IEventHandler<TEvent> handler)
            {
                try
                {
                    return handler.HandleAsync(@event, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Exception thrown during event dispatch: {ExceptionDetailMessage}", exception.DetailMessage(true));
                    (exceptions ??= new List<Exception>()).Add(exception);

                    return Task.FromException(exception);
                }
            }
        }

        #region Nested types
        private class ReflectedQueryHandler<TResult>
        {
            private readonly object handlerInstance;
            private readonly MethodInfo handleMethod;

            public ReflectedQueryHandler(Type queryType, IServiceProvider serviceProvider)
            {
                const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;

                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
                handlerInstance = serviceProvider.GetRequiredService(handlerType);
                handleMethod = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync), bindingAttr);
            }

            [DebuggerStepThrough]
            public Task<TResult> InvokeAsync(object query, object cancellationToken)
            {
                try
                {
                    return (Task<TResult>)handleMethod.Invoke(handlerInstance, new[] { query, cancellationToken });
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                    return Task.FromException<TResult>(exception.InnerException); // Redundant, but compiler cannot infer ExceptionDispatchInfo.Throw() will actually throw.
                }
            }
        }

        private class ReflectedQueryHandlerWrappers<TResult> : IEnumerable<object>
        {
            private readonly IEnumerable<object> wrapperInstances;
            private readonly MethodInfo wrapperHandleMethod;

            public ReflectedQueryHandlerWrappers(Type queryType, IServiceProvider serviceProvider)
            {
                const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;

                var wrapperType = typeof(IQueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult));
                wrapperInstances = serviceProvider.GetServices(wrapperType);
                wrapperHandleMethod = wrapperType.GetMethod(nameof(IQueryHandlerWrapper<IQuery<TResult>, TResult>.HandleAsync), bindingAttr);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<object> GetEnumerator() => wrapperInstances.GetEnumerator();

            [DebuggerStepThrough]
            public Task<TResult> InvokeAsync(object wrapperInstance, object query, object next, object cancellationToken)
            {
                try
                {
                    return (Task<TResult>)wrapperHandleMethod.Invoke(wrapperInstance, new[] { query, next, cancellationToken });
                }
                catch (TargetInvocationException exception)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                    return Task.FromException<TResult>(exception.InnerException); // Redundant, but compiler cannot infer ExceptionDispatchInfo.Throw() will actually throw.
                }
            }
        }
        #endregion
    }
}