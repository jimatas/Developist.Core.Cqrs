using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static partial class CqrsBuilderExtensions
    {
        public static CqrsBuilder AddDefaultDispatcher(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDispatcher), typeof(DefaultDispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(ICommandDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IEventDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IQueryDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));

            return builder;
        }

        public static CqrsBuilder AddDefaultRegistry(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(DefaultRegistry), typeof(DefaultRegistry), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IHandlerRegistry), provider => provider.GetRequiredService<DefaultRegistry>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IInterceptorRegistry), provider => provider.GetRequiredService<DefaultRegistry>(), lifetime));

            return builder;
        }

        public static CqrsBuilder AddHandlersFromAssembly(this CqrsBuilder builder, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
                            builder.Services.Add(new ServiceDescriptor(implementedInterface, type, lifetime));
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            builder.Services.Add(new ServiceDescriptor(openGenericInterface, type, lifetime));
                        }
                        else
                        {
                            throw new InvalidOperationException("Types that only partially close the IQueryHandler generic interface are not supported.");
                        }
                    }
                }
            }

            return builder;
        }

        public static CqrsBuilder AddInterceptorsFromAssembly(this CqrsBuilder builder, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
                            builder.Services.Add(new ServiceDescriptor(implementedInterface, type, lifetime));
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            builder.Services.Add(new ServiceDescriptor(openGenericInterface, type, lifetime));
                        }
                        else
                        {
                            throw new InvalidOperationException("Types that only partially close the IQueryInterceptor generic interface are not supported.");
                        }
                    }
                }
            }

            return builder;
        }
    }
}
