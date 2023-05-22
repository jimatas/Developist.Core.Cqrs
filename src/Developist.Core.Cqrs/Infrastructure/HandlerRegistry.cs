using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IHandlerRegistry"/> interface, which retrieves handlers and interceptors using the built-in DI container.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class HandlerRegistry : IHandlerRegistry
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerRegistry"/> class with the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to use for retrieving handlers and interceptors.</param>
        public HandlerRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public ICommandHandler<TCommand> GetCommandHandler<TCommand>()
            where TCommand : ICommand
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand));
            var handlers = _serviceProvider.GetServices(handlerType);

            return handlers.Count() == 1
                ? (ICommandHandler<TCommand>)handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type '{typeof(TCommand)}'.");
        }

        /// <inheritdoc/>
        public IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>() where TCommand : ICommand
        {
            var interceptorType = typeof(ICommandInterceptor<>).MakeGenericType(typeof(TCommand));
            var interceptors = _serviceProvider.GetServices(interceptorType).Cast<ICommandInterceptor<TCommand>>();

            return interceptors.OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }

        /// <inheritdoc/>
        public IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
            where TEvent : IEvent
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));

            return _serviceProvider.GetServices(handlerType).Cast<IEventHandler<TEvent>>();
        }

        /// <inheritdoc/>
        public IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
            var handlers = _serviceProvider.GetServices(handlerType);

            return handlers.Count() == 1
                ? (IQueryHandler<TQuery, TResult>)handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type '{typeof(TQuery)}' and result type '{typeof(TResult)}'.");
        }

        /// <inheritdoc/>
        public IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            var interceptorType = typeof(IQueryInterceptor<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
            var interceptors = _serviceProvider.GetServices(interceptorType).Cast<IQueryInterceptor<TQuery, TResult>>();

            return interceptors.OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }
    }
}
