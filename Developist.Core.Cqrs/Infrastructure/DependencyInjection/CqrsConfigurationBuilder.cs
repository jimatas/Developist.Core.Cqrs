using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public sealed class CqrsConfigurationBuilder : IDispatcherConfiguration, IRegistryConfiguration, IHandlerConfiguration, IInterceptorConfiguration
    {
        internal CqrsConfigurationBuilder(IServiceCollection services) => Services = services;

        public IServiceCollection Services { get; }

        IRegistryConfiguration IDispatcherConfiguration.AddDefaultDispatcher(ServiceLifetime lifetime)
        {
            Services.Add(new ServiceDescriptor(typeof(IDispatcher), typeof(DefaultDispatcher), lifetime));
            Services.Add(new ServiceDescriptor(typeof(ICommandDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            Services.Add(new ServiceDescriptor(typeof(IEventDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            Services.Add(new ServiceDescriptor(typeof(IQueryDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));

            return this;
        }

        IHandlerConfiguration IRegistryConfiguration.AddDefaultRegistry(ServiceLifetime lifetime)
        {
            Services.Add(new ServiceDescriptor(typeof(DefaultRegistry), typeof(DefaultRegistry), lifetime));
            Services.Add(new ServiceDescriptor(typeof(IHandlerRegistry), provider => provider.GetRequiredService<DefaultRegistry>(), lifetime));
            Services.Add(new ServiceDescriptor(typeof(IInterceptorRegistry), provider => provider.GetRequiredService<DefaultRegistry>(), lifetime));

            return this;
        }

        IInterceptorConfiguration IHandlerConfiguration.AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => assembly);

            foreach (var openGenericInterface in new[] { typeof(ICommandHandler<>), typeof(IEventHandler<>), typeof(IQueryHandler<,>) })
            {
                foreach (var type in assembly.ExportedTypes.Where(type => type.IsConcrete()))
                {
                    foreach (var implementedInterface in type.GetImplementedGenericInterfaces(openGenericInterface))
                    {
                        if (!type.IsGenericType)
                        {
                            Services.Add(new ServiceDescriptor(implementedInterface, type, lifetime));
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            Services.Add(new ServiceDescriptor(openGenericInterface, type, lifetime));
                        }
                        else
                        {
                            throw new InvalidOperationException("Types that only partially close the IQueryHandler generic interface are not supported.");
                        }
                    }
                }
            }

            return this;
        }

        IInterceptorConfiguration IInterceptorConfiguration.AddInterceptorsFromAssembly(Assembly assembly, ServiceLifetime lifetime)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => assembly);

            foreach (var openGenericInterface in new[] { typeof(ICommandInterceptor<>), typeof(IQueryInterceptor<,>) })
            {
                foreach (var type in assembly.ExportedTypes.Where(type => type.IsConcrete()))
                {
                    foreach (var implementedInterface in type.GetImplementedGenericInterfaces(openGenericInterface))
                    {
                        if (!type.IsGenericType)
                        {
                            Services.Add(new ServiceDescriptor(implementedInterface, type, lifetime));
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            Services.Add(new ServiceDescriptor(openGenericInterface, type, lifetime));
                        }
                        else
                        {
                            throw new InvalidOperationException("Types that only partially close the IQueryInterceptor generic interface are not supported.");
                        }
                    }
                }
            }

            return this;
        }
    }
}
