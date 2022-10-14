using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    public sealed class HandlerRegistry : IHandlerRegistry
    {
        private readonly IServiceProvider serviceProvider;

        public HandlerRegistry(IServiceProvider serviceProvider)
        {
            this.serviceProvider = ArgumentNullExceptionHelper.ThrowIfNull(() => serviceProvider);
        }

        public object GetCommandHandler(Type commandType)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handlers = serviceProvider.GetServices(handlerType);
            return handlers.Count() == 1 ? handlers.Single()!
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type '{commandType}'.");
        }

        public IEnumerable<object> GetCommandInterceptors(Type commandType)
        {
            var interceptorType = typeof(ICommandInterceptor<>).MakeGenericType(commandType);
            return serviceProvider.GetServices(interceptorType)!;
        }

        public IEnumerable<object> GetEventHandlers(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return serviceProvider.GetServices(handlerType)!;
        }

        public object GetQueryHandler(Type queryType, Type resultType)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
            var handlers = serviceProvider.GetServices(handlerType);
            return handlers.Count() == 1 ? handlers.Single()!
                : throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type '{queryType}' and result type '{resultType}'.");
        }

        public IEnumerable<object> GetQueryInterceptors(Type queryType, Type resultType)
        {
            var interceptorType = typeof(IQueryInterceptor<,>).MakeGenericType(queryType, resultType);
            return serviceProvider.GetServices(interceptorType)!;
        }
    }
}
