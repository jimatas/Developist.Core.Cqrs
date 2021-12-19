// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure
{
    public class DefaultHandlerRegistry : IHandlerRegistry
    {
        private readonly ICollection<Type> handlerAndWrapperTypes;

        private DefaultHandlerRegistry(ICollection<Type> handlerAndWrapperTypes)
            => this.handlerAndWrapperTypes = handlerAndWrapperTypes;

        public static DefaultHandlerRegistry InitializeFromAssembly(Assembly assembly)
        {
            Ensure.Argument.NotNull(assembly, nameof(assembly));

            return InitializeFromAssemblies(new[] { assembly });
        }

        public static DefaultHandlerRegistry InitializeFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            Ensure.Argument.NotNull(assemblies, nameof(assemblies));
            Ensure.Argument.NotNullOrEmpty(assemblies, nameof(assemblies), "There must be at least one assembly provided.");

            var handlerAndWrapperTypes = assemblies.SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.IsConcrete())
                .Where(type => type.ImplementsGenericInterface(typeof(ICommandHandler<>))
                    || type.ImplementsGenericInterface(typeof(ICommandHandlerWrapper<>))
                    || type.ImplementsGenericInterface(typeof(IQueryHandler<,>))
                    || type.ImplementsGenericInterface(typeof(IQueryHandlerWrapper<,>))
                    || type.ImplementsGenericInterface(typeof(IEventHandler<>)));

            return new DefaultHandlerRegistry(new HashSet<Type>(handlerAndWrapperTypes));
        }

        public object GetCommandHandler(Type commandType)
        {
            ICollection<object> handlers = new List<object>();
            foreach (var handlerType in handlerAndWrapperTypes.Where(type => type.ImplementsGenericInterface(typeof(ICommandHandler<>))))
            {
                foreach (var typeArgument in handlerType.GetImplementedGenericInterfaces(typeof(ICommandHandler<>)).Select(iface => iface.GetGenericArguments().Single()))
                {
                    if (typeArgument == commandType)
                    {
                        handlers.Add(Activator.CreateInstance(handlerType));
                    }
                }
            }

            if (handlers.Count == 1)
            {
                return handlers.Single();
            }
            throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for command with type {commandType}.");
        }

        public IEnumerable<object> GetCommandHandlerWrappers(Type commandType)
        {
            ICollection<object> wrappers = new List<object>();
            foreach (var handlerType in handlerAndWrapperTypes.Where(type => type.ImplementsGenericInterface(typeof(ICommandHandlerWrapper<>))))
            {
                foreach (var typeArgument in handlerType.GetImplementedGenericInterfaces(typeof(ICommandHandlerWrapper<>)).Select(iface => iface.GetGenericArguments().Single()))
                {
                    if (typeArgument == commandType)
                    {
                        wrappers.Add(Activator.CreateInstance(handlerType));
                    }
                    else if (typeArgument.IsGenericParameter)
                    {
                        wrappers.Add(Activator.CreateInstance(handlerType.MakeGenericType(commandType)));
                    }
                }
            }
            return wrappers;
        }

        public object GetQueryHandler(Type queryType, Type resultType)
        {
            ICollection<object> handlers = new List<object>();
            foreach (var handlerType in handlerAndWrapperTypes.Where(type => type.ImplementsGenericInterface(typeof(IQueryHandler<,>))))
            {
                foreach (var typeArguments in handlerType.GetImplementedGenericInterfaces(typeof(IQueryHandler<,>)).Select(iface => iface.GetGenericArguments()))
                {
                    if (typeArguments.First() == queryType && typeArguments.Last() == resultType)
                    {
                        handlers.Add(Activator.CreateInstance(handlerType));
                    }
                }
            }

            if (handlers.Count == 1)
            {
                return handlers.Single();
            }
            throw new InvalidOperationException($"{(handlers.Any() ? "More than one" : "No")} handler found for query with type {queryType}.");
        }

        public IEnumerable<object> GetQueryHandlerWrappers(Type queryType, Type resultType)
        {
            ICollection<object> wrappers = new List<object>();
            foreach (var wrapperType in handlerAndWrapperTypes.Where(type => type.ImplementsGenericInterface(typeof(IQueryHandlerWrapper<,>))))
            {
                foreach (var typeArguments in wrapperType.GetImplementedGenericInterfaces(typeof(IQueryHandlerWrapper<,>)).Select(iface => iface.GetGenericArguments()))
                {
                    if (typeArguments.First() == queryType && typeArguments.Last() == resultType)
                    {
                        wrappers.Add(Activator.CreateInstance(wrapperType));
                    }
                    else if (typeArguments.First().IsGenericParameter)
                    {
                        if (typeArguments.Last().IsGenericParameter)
                        {
                            wrappers.Add(Activator.CreateInstance(wrapperType.MakeGenericType(queryType, resultType)));
                        }
                        else
                        {
                            wrappers.Add(Activator.CreateInstance(wrapperType.MakeGenericType(queryType)));
                        }
                    }
                }
            }
            return wrappers;
        }

        public IEnumerable<object> GetEventHandlers(Type eventType)
        {
            ICollection<object> handlers = new List<object>();
            foreach (var eventHandlerType in handlerAndWrapperTypes.Where(type => type.ImplementsGenericInterface(typeof(IEventHandler<>))))
            {
                foreach (var typeArgument in eventHandlerType.GetImplementedGenericInterfaces(typeof(IEventHandler<>)).Select(iface => iface.GetGenericArguments().Single()))
                {
                    if (typeArgument == eventType)
                    {
                        handlers.Add(Activator.CreateInstance(eventHandlerType));
                    }
                }
            }
            return handlers;
        }
    }
}
