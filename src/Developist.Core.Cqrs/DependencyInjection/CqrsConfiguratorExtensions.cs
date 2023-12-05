using Developist.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring CQRS-related services using a <see cref="CqrsConfigurator"/>.
/// </summary>
public static class CqrsConfiguratorExtensions
{
    /// <summary>
    /// Adds the default dispatcher and related services to the CQRS configuration.
    /// </summary>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddDefaultDispatcher(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var service = new ServiceDescriptor(typeof(IDispatcher), typeof(DefaultDispatcher), lifetime);
        configurator.Services.TryAdd(service);

        service = new(typeof(ICommandDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime);
        configurator.Services.TryAdd(service);

        service = new(typeof(IQueryDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime);
        configurator.Services.TryAdd(service);

        service = new(typeof(IEventDispatcher), provider => provider.GetRequiredService<IDispatcher>(), lifetime);
        configurator.Services.TryAdd(service);

        service = new(typeof(IHandlerRegistry), typeof(DefaultHandlerRegistry), lifetime);
        configurator.Services.TryAdd(service);

        return configurator;
    }

    /// <summary>
    /// Adds a command handler to the CQRS configuration.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle.</typeparam>
    /// <typeparam name="TCommandHandler">The type of command handler.</typeparam>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddCommandHandler<TCommand, TCommandHandler>(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>
    {
        var service = new ServiceDescriptor(typeof(ICommandHandler<TCommand>), typeof(TCommandHandler), lifetime);
        configurator.Services.TryAdd(service);

        return configurator;
    }

    /// <summary>
    /// Adds a command interceptor to the CQRS configuration.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to intercept.</typeparam>
    /// <typeparam name="TCommandInterceptor">The type of command interceptor.</typeparam>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddCommandInterceptor<TCommand, TCommandInterceptor>(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TCommand : ICommand
        where TCommandInterceptor : ICommandInterceptor<TCommand>
    {
        var service = new ServiceDescriptor(typeof(ICommandInterceptor<TCommand>), typeof(TCommandInterceptor), lifetime);
        configurator.Services.TryAddEnumerable(service);

        return configurator;
    }

    /// <summary>
    /// Adds a query handler to the CQRS configuration.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
    /// <typeparam name="TQueryHandler">The type of query handler.</typeparam>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddQueryHandler<TQuery, TResult, TQueryHandler>(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TQuery : IQuery<TResult>
        where TQueryHandler : IQueryHandler<TQuery, TResult>
    {
        var service = new ServiceDescriptor(typeof(IQueryHandler<TQuery, TResult>), typeof(TQueryHandler), lifetime);
        configurator.Services.TryAdd(service);

        return configurator;
    }

    /// <summary>
    /// Adds a query interceptor to the CQRS configuration.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to intercept.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
    /// <typeparam name="TQueryInterceptor">The type of query interceptor.</typeparam>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddQueryInterceptor<TQuery, TResult, TQueryInterceptor>(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TQuery : IQuery<TResult>
        where TQueryInterceptor : IQueryInterceptor<TQuery, TResult>
    {
        var service = new ServiceDescriptor(typeof(IQueryInterceptor<TQuery, TResult>), typeof(TQueryInterceptor), lifetime);
        configurator.Services.TryAddEnumerable(service);

        return configurator;
    }

    /// <summary>
    /// Adds an event handler to the CQRS configuration.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to handle.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    public static CqrsConfigurator AddEventHandler<TEvent, TEventHandler>(this CqrsConfigurator configurator, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var service = new ServiceDescriptor(typeof(IEventHandler<TEvent>), typeof(TEventHandler), lifetime);
        configurator.Services.TryAddEnumerable(service);

        return configurator;
    }

    /// <summary>
    /// Adds command, query, and event handlers, as well as interceptors, from an assembly to the CQRS configuration.
    /// </summary>
    /// <param name="configurator">The <see cref="CqrsConfigurator"/> to which services will be added.</param>
    /// <param name="assembly">The assembly from which handlers and interceptors will be discovered and added.</param>
    /// <param name="lifetime">The service lifetime for added services. Defaults to scoped.</param>
    /// <returns>The <see cref="CqrsConfigurator"/> with services added.</returns>
    /// <exception cref="InvalidOperationException"/>
    public static CqrsConfigurator AddHandlersFromAssembly(this CqrsConfigurator configurator, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        var handlerInterfaces = new[]
        {
            (typeof(ICommandHandler<>), false),
            (typeof(ICommandInterceptor<>), true),
            (typeof(IQueryHandler<,>), false),
            (typeof(IQueryInterceptor<,>), true),
            (typeof(IEventHandler<>), true)
        };

        foreach (var type in assembly.ExportedTypes.Where(type => type.IsConcrete()))
        {
            foreach (var (openGenericInterface, isEnumerable) in handlerInterfaces)
            {
                foreach (var implementedInterface in type.GetImplementedGenericInterfaces(openGenericInterface))
                {
                    ServiceDescriptor service;
                    if (!type.IsGenericType)
                    {
                        service = new(implementedInterface, type, lifetime);
                    }
                    else if (implementedInterface.GetGenericArguments().All(arg => arg.IsGenericParameter))
                    {
                        service = new(openGenericInterface, type, lifetime);
                    }
                    else
                    {
                        throw new InvalidOperationException("Types that partially close the "
                            + $"{nameof(IQueryHandler<IQuery<object>, object>)} or "
                            + $"{nameof(IQueryInterceptor<IQuery<object>, object>)} generic interfaces are not supported.");
                    }

                    if (isEnumerable)
                    {
                        configurator.Services.TryAddEnumerable(service);
                    }
                    else
                    {
                        configurator.Services.TryAdd(service);
                    }
                }
            }
        }

        return configurator;
    }
}
