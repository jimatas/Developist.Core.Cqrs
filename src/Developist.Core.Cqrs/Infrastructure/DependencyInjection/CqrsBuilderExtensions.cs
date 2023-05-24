using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for configuring CQRS services using the <see cref="CqrsBuilder"/> class.
    /// </summary>
    public static class CqrsBuilderExtensions
    {
        /// <summary>
        /// Adds the dispatcher classes and related interfaces and registry services to the service collection.
        /// </summary>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddDispatchers(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IDispatcher), typeof(Dispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(ICommandDispatcher), typeof(CommandDispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IEventDispatcher), typeof(EventDispatcher), lifetime));
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IQueryDispatcher), typeof(QueryDispatcher), lifetime));

            builder.Services.TryAdd(new ServiceDescriptor(typeof(IHandlerRegistry), typeof(HandlerRegistry), lifetime));

            return builder;
        }

        /// <summary>
        /// Adds command, event, and query handlers, along with interceptors, from the specified assembly to the service collection.
        /// </summary>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="assembly">The assembly to scan for handler types.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddHandlersFromAssembly(this CqrsBuilder builder, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

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
                            throw new InvalidOperationException($"Types that only partially close either the {nameof(IQueryHandler<IQuery<object>, object>)} or {nameof(IQueryInterceptor<IQuery<object>, object>)} generic interface are not supported.");
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

        /// <summary>
        /// Adds a command handler to the service collection.
        /// </summary>
        /// <typeparam name="TCommand">The type of command.</typeparam>
        /// <typeparam name="TCommandHandler">The type of command handler.</typeparam>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddCommandHandler<TCommand, TCommandHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
            where TCommandHandler : ICommandHandler<TCommand>
        {
            var service = new ServiceDescriptor(typeof(ICommandHandler<TCommand>), typeof(TCommandHandler), lifetime);
            builder.Services.TryAdd(service);

            return builder;
        }

        /// <summary>
        /// Adds a command interceptor to the service collection.
        /// </summary>
        /// <typeparam name="TCommand">The type of command.</typeparam>
        /// <typeparam name="TCommandInterceptor">The type of command interceptor.</typeparam>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddCommandInterceptor<TCommand, TCommandInterceptor>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
            where TCommandInterceptor : ICommandInterceptor<TCommand>
        {
            var service = new ServiceDescriptor(typeof(ICommandInterceptor<TCommand>), typeof(TCommandInterceptor), lifetime);
            builder.Services.TryAddEnumerable(service);

            return builder;
        }

        /// <summary>
        /// Adds an event handler to the service collection.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddEventHandler<TEvent, TEventHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var service = new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), lifetime);
            builder.Services.TryAddEnumerable(service);

            return builder;
        }

        /// <summary>
        /// Adds a query handler to the service collection.
        /// </summary>
        /// <typeparam name="TQuery">The type of query.</typeparam>
        /// <typeparam name="TResult">The type of query result.</typeparam>
        /// <typeparam name="TQueryHandler">The type of query handler.</typeparam>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
        public static CqrsBuilder AddQueryHandler<TQuery, TResult, TQueryHandler>(this CqrsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
            where TQueryHandler : IQueryHandler<TQuery, TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryHandler<TQuery, TResult>), typeof(TQueryHandler), lifetime);
            builder.Services.TryAdd(service);

            return builder;
        }

        /// <summary>
        /// Adds a query interceptor to the service collection.
        /// </summary>
        /// <typeparam name="TQuery">The type of query.</typeparam>
        /// <typeparam name="TResult">The type of query result.</typeparam>
        /// <typeparam name="TQueryInterceptor">The type of query interceptor.</typeparam>
        /// <param name="builder">The <see cref="CqrsBuilder"/> instance to add the services to.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The <see cref="CqrsBuilder"/> instance with added services.</returns>
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
