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
        public static CqrsBuilder AddDispatcher(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDispatcher), typeof(Dispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(ICommandDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IEventDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IQueryDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime));

            builder.Services.TryAdd(new ServiceDescriptor(typeof(IHandlerRegistry), typeof(HandlerRegistry), lifetime));

            return builder;
        }

        public static CqrsBuilder AddDynamicDispatcher(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDynamicDispatcher), typeof(DynamicDispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDynamicCommandDispatcher), provider => provider.GetRequiredService<IDynamicDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDynamicEventDispatcher), provider => provider.GetRequiredService<IDynamicDispatcher>(), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDynamicQueryDispatcher), provider => provider.GetRequiredService<IDynamicDispatcher>(), lifetime));

            builder.Services.TryAdd(new ServiceDescriptor(typeof(IHandlerRegistry), typeof(HandlerRegistry), lifetime));

            return builder;
        }

        public static CqrsBuilder AddHandlersFromAssembly(this CqrsBuilder builder, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => assembly);

            foreach (var (openGenericInterface, isEnumerable) in new[]
            {
                (typeof(ICommandHandler<>), false),
                (typeof(ICommandInterceptor<>), true),
                (typeof(IEventHandler<>), true),
                (typeof(IQueryHandler<,>), false),
                (typeof(IQueryInterceptor<,>), true)
            })
            {
                foreach (var type in assembly.ExportedTypes.Where(type => type.IsConcrete()))
                {
                    foreach (var implementedInterface in type.GetImplementedGenericInterfaces(openGenericInterface))
                    {
                        ServiceDescriptor service;
                        if (!type.IsGenericType)
                        {
                            service = new ServiceDescriptor(implementedInterface, type, lifetime);
                        }
                        else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                        {
                            service = new ServiceDescriptor(openGenericInterface, type, lifetime);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Types that only partially close the {nameof(IQueryHandler<IQuery<object>, object>)} or {nameof(IQueryInterceptor<IQuery<object>, object>)} generic interface are not supported.");
                        }

                        if (isEnumerable)
                        {
                            builder.Services.TryAddEnumerable(service);
                        }
                        else
                        {
                            builder.Services.TryAdd(service);
                        }
                    }
                }
            }

            return builder;
        }

        public static CqrsBuilder AddCommandHandler<TCommand, TCommandHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
            where TCommandHandler : ICommandHandler<TCommand>
        {
            var service = new ServiceDescriptor(typeof(ICommandHandler<TCommand>), typeof(TCommandHandler), lifetime);
            builder.Services.TryAdd(service);
            return builder;
        }

        public static CqrsBuilder AddCommandInterceptor<TCommand, TCommandInterceptor>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
            where TCommandInterceptor : ICommandInterceptor<TCommand>
        {
            var service = new ServiceDescriptor(typeof(ICommandInterceptor<TCommand>), typeof(TCommandInterceptor), lifetime);
            builder.Services.TryAddEnumerable(service);
            return builder;
        }

        public static CqrsBuilder AddEventHandler<TEvent, TEventHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var service = new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), lifetime);
            builder.Services.TryAddEnumerable(service);
            return builder;
        }

        public static CqrsBuilder AddQueryHandler<TQuery, TResult, TQueryHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
            where TQueryHandler : IQueryHandler<TQuery, TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryHandler<TQuery, TResult>), typeof(TQueryHandler), lifetime);
            builder.Services.TryAdd(service);
            return builder;
        }

        public static CqrsBuilder AddQueryInterceptor<TQuery, TResult, TQueryInterceptor>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
            where TQueryInterceptor : IQueryInterceptor<TQuery, TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryInterceptor<TQuery, TResult>), typeof(TQueryInterceptor), lifetime);
            builder.Services.TryAddEnumerable(service);
            return builder;
        }
    }
}
