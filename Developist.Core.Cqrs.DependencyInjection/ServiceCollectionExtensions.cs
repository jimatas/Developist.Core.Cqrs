// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDispatcher(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add(new ServiceDescriptor(typeof(IDispatcher), typeof(Dispatcher), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ICommandDispatcher), provider => provider.GetService<IDispatcher>(), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IQueryDispatcher), provider => provider.GetService<IDispatcher>(), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IEventDispatcher), provider => provider.GetService<IDispatcher>(), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IHandlerRegistry), typeof(ServiceProviderHandlerRegistry), serviceLifetime));

            return services;
        }

        public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            Ensure.Argument.NotNull(assembly, nameof(assembly));

            foreach (var openGenericInterface in new[] {
                typeof(ICommandHandler<>),
                typeof(ICommandHandlerWrapper<>),
                typeof(IQueryHandler<,>),
                typeof(IQueryHandlerWrapper<,>),
                typeof(IEventHandler<>) })
            {
                foreach (var type in assembly.ExportedTypes.Where(type => type.IsConcrete()))
                {
                    foreach (var implementedInterface in type.GetImplementedGenericInterfaces(openGenericInterface))
                    {
                        if (!type.IsGenericType)
                        {
                            services.Add(new ServiceDescriptor(implementedInterface, type, serviceLifetime));
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            services.Add(new ServiceDescriptor(openGenericInterface, type, serviceLifetime));
                        }
                        else
                        {
                            throw new InvalidOperationException("Partially closed generic handlers are not supported.");
                        }
                    }
                }
            }

            return services;
        }
    }
}
