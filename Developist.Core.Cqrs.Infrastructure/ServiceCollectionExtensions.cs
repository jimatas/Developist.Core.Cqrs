// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services,
            ServiceLifetime dispatcherLifetime = ServiceLifetime.Scoped,
            ServiceLifetime handlerLifetime = ServiceLifetime.Scoped,
            params Assembly[] handlerAssemblies)
        {
            if (handlerAssemblies is null || !handlerAssemblies.Any())
            {
                handlerAssemblies = new[] { Assembly.GetCallingAssembly() };
            }
            else
            {
                handlerAssemblies = handlerAssemblies.Distinct().ToArray();
            }

            services.Add(new ServiceDescriptor(typeof(IDispatcher), typeof(Dispatcher), dispatcherLifetime));
            services.Add(new ServiceDescriptor(typeof(ICommandDispatcher), provider => provider.GetService<IDispatcher>(), dispatcherLifetime));
            services.Add(new ServiceDescriptor(typeof(IQueryDispatcher), provider => provider.GetService<IDispatcher>(), dispatcherLifetime));
            services.Add(new ServiceDescriptor(typeof(IEventDispatcher), provider => provider.GetService<IDispatcher>(), dispatcherLifetime));

            services.FromAssemblies(handlerAssemblies)
                .AddClasses(type => type.GetInterfaces().Any(iface => iface.IsGenericType && (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || iface.GetGenericTypeDefinition() == typeof(ICommandHandlerWrapper<>))))
                .AsImplementedInterfaces()
                .WithLifetime(handlerLifetime);

            services.FromAssemblies(handlerAssemblies)
                .AddClasses(type => type.GetInterfaces().Any(iface => iface.IsGenericType && (iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) || iface.GetGenericTypeDefinition() == typeof(IQueryHandlerWrapper<,>))))
                .AsImplementedInterfaces()
                .WithLifetime(handlerLifetime);

            services.FromAssemblies(handlerAssemblies)
                .AddClasses(type => type.GetInterfaces().Any(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithLifetime(handlerLifetime);

            AddOpenGenericHandlers();
            AddOpenGenericWrappers();

            return services;

            void AddOpenGenericHandlers()
            {
                foreach (var openGenericInterface in new[] { typeof(ICommandHandler<>), typeof(IQueryHandler<,>) })
                {
                    foreach (var openGenericImplementation in handlerAssemblies.SelectMany(assembly => assembly.ExportedTypes)
                        .Where(type => type.IsConcrete() && type.IsOpenGeneric() && type.FindGenericInterfaces(openGenericInterface).Any()))
                    {
                        services.Add(new ServiceDescriptor(openGenericInterface, openGenericImplementation, handlerLifetime));
                    }
                }
            }

            void AddOpenGenericWrappers()
            {
                foreach (var openGenericInterface in new[] { typeof(ICommandHandlerWrapper<>), typeof(IQueryHandlerWrapper<,>) })
                {
                    foreach (var openGenericImplementation in handlerAssemblies.SelectMany(assembly => assembly.ExportedTypes)
                        .Where(type => type.IsConcrete() && type.IsOpenGeneric() && type.FindGenericInterfaces(openGenericInterface).Any()))
                    {
                        services.Add(new ServiceDescriptor(openGenericInterface, openGenericImplementation, handlerLifetime));
                    }
                }
            }
        }

        internal static AssemblySelector FromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies) => new(services, assemblies);
    }
}
