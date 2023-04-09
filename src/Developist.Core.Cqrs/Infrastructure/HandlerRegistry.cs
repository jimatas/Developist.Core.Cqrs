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
        /// Initializes a new instance of the <see cref="HandlerRegistry"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to use for retrieving handlers and interceptors.</param>
        public HandlerRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public object GetCommandHandler(Type commandType)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handlers = _serviceProvider.GetServices(handlerType);
            return handlers.Count() == 1 ? handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type '{commandType}'.");
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetCommandInterceptors(Type commandType)
        {
            var interceptorType = typeof(ICommandInterceptor<>).MakeGenericType(commandType);
            return _serviceProvider.GetServices(interceptorType).OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetEventHandlers(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return _serviceProvider.GetServices(handlerType);
        }

        /// <inheritdoc/>
        public object GetQueryHandler(Type queryType, Type resultType)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
            var handlers = _serviceProvider.GetServices(handlerType);
            return handlers.Count() == 1 ? handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type '{queryType}' and result type '{resultType}'.");
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetQueryInterceptors(Type queryType, Type resultType)
        {
            var interceptorType = typeof(IQueryInterceptor<,>).MakeGenericType(queryType, resultType);
            return _serviceProvider.GetServices(interceptorType).OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }
    }
}
