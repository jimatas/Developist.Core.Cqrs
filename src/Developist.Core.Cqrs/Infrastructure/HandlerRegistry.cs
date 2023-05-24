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
            var handlers = _serviceProvider.GetServices<ICommandHandler<TCommand>>();
            return handlers.Count() == 1 ? handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type '{typeof(TCommand)}'.");
        }

        /// <inheritdoc/>
        public IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>()
            where TCommand : ICommand
        {
            return _serviceProvider.GetServices<ICommandInterceptor<TCommand>>()
                .OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }

        /// <inheritdoc/>
        public IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
            where TEvent : IEvent
        {
            return _serviceProvider.GetServices<IEventHandler<TEvent>>();
        }

        /// <inheritdoc/>
        public IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            var handlers = _serviceProvider.GetServices<IQueryHandler<TQuery, TResult>>();
            return handlers.Count() == 1 ? handlers.Single()
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type '{typeof(TQuery)}' and result type '{typeof(TResult)}'.");
        }

        /// <inheritdoc/>
        public IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            return _serviceProvider.GetServices<IQueryInterceptor<TQuery, TResult>>()
                .OrderBy(interceptor => (interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal);
        }
    }
}
