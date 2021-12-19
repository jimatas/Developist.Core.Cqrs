// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.DependencyInjection
{
    public class ServiceProviderHandlerRegistry : IHandlerRegistry
    {
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderHandlerRegistry(IServiceProvider serviceProvider)
            => this.serviceProvider = Ensure.Argument.NotNull(serviceProvider, nameof(serviceProvider));

        public object GetCommandHandler(Type commandType)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handlers = serviceProvider.GetServices(handlerType);
            if (handlers.Count() == 1)
            {
                return handlers.Single();
            }
            throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type {commandType}.");
        }

        public IEnumerable<object> GetCommandHandlerWrappers(Type commandType)
        {
            var wrapperType = typeof(ICommandHandlerWrapper<>).MakeGenericType(commandType);
            return serviceProvider.GetServices(wrapperType);
        }

        public object GetQueryHandler(Type queryType, Type resultType)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
            var handlers = serviceProvider.GetServices(handlerType);
            if (handlers.Count() == 1)
            {
                return handlers.Single();
            }
            throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type {queryType}.");
        }

        public IEnumerable<object> GetQueryHandlerWrappers(Type queryType, Type resultType)
        {
            var wrapperType = typeof(IQueryHandlerWrapper<,>).MakeGenericType(queryType, resultType);
            return serviceProvider.GetServices(wrapperType);
        }

        public IEnumerable<object> GetEventHandlers(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return serviceProvider.GetServices(handlerType);
        }
    }
}
