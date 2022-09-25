using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static class CqrsConfigurationBuilderExtensions
    {
        public static IInterceptorConfiguration AddCommandHandler<TCommand>(this IHandlerConfiguration configuration, Func<TCommand, CancellationToken, Task> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
        {
            var service = new ServiceDescriptor(typeof(ICommandHandler<TCommand>), _ => new DelegatingCommandHandler<TCommand>(handleAsync), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddCommandHandler<TCommand>(this IHandlerConfiguration configuration, Func<TCommand, CancellationToken, IServiceProvider, Task> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
        {
            var service = new ServiceDescriptor(typeof(ICommandHandler<TCommand>), provider => new DelegatingCommandHandler<TCommand>(handleAsync, provider), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddCommandInterceptor<TCommand>(this IInterceptorConfiguration configuration, Func<TCommand, HandlerDelegate, CancellationToken, Task> interceptAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
        {
            var service = new ServiceDescriptor(typeof(ICommandInterceptor<TCommand>), _ => new DelegatingCommandInterceptor<TCommand>(interceptAsync), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return configuration;
        }

        public static IInterceptorConfiguration AddCommandInterceptor<TCommand>(this IInterceptorConfiguration configuration, Func<TCommand, HandlerDelegate, CancellationToken, IServiceProvider, Task> interceptAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TCommand : ICommand
        {
            var service = new ServiceDescriptor(typeof(ICommandInterceptor<TCommand>), provider => new DelegatingCommandInterceptor<TCommand>(interceptAsync, provider), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return configuration;
        }

        public static IInterceptorConfiguration AddEventHandler<TEvent>(this IHandlerConfiguration configuration, Func<TEvent, CancellationToken, Task> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEvent : IEvent
        {
            var service = new ServiceDescriptor(typeof(IEventHandler<TEvent>), _ => new DelegatingEventHandler<TEvent>(handleAsync), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddEventHandler<TEvent>(this IHandlerConfiguration configuration, Func<TEvent, CancellationToken, IServiceProvider, Task> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEvent : IEvent
        {
            var service = new ServiceDescriptor(typeof(IEventHandler<TEvent>), provider => new DelegatingEventHandler<TEvent>(handleAsync, provider), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddQueryHandler<TQuery, TResult>(this IHandlerConfiguration configuration, Func<TQuery, CancellationToken, Task<TResult>> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryHandler<TQuery, TResult>), _ => new DelegatingQueryHandler<TQuery, TResult>(handleAsync), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddQueryHandler<TQuery, TResult>(this IHandlerConfiguration configuration, Func<TQuery, CancellationToken, IServiceProvider, Task<TResult>> handleAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryHandler<TQuery, TResult>), provider => new DelegatingQueryHandler<TQuery, TResult>(handleAsync, provider), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return (IInterceptorConfiguration)configuration;
        }

        public static IInterceptorConfiguration AddQueryInterceptor<TQuery, TResult>(this IInterceptorConfiguration configuration, Func<TQuery, HandlerDelegate<TResult>, CancellationToken, Task<TResult>> interceptAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryInterceptor<TQuery, TResult>), _ => new DelegatingQueryInterceptor<TQuery, TResult>(interceptAsync), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return configuration;
        }

        public static IInterceptorConfiguration AddQueryInterceptor<TQuery, TResult>(this IInterceptorConfiguration configuration, Func<TQuery, HandlerDelegate<TResult>, CancellationToken, IServiceProvider, Task<TResult>> interceptAsync, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TQuery : IQuery<TResult>
        {
            var service = new ServiceDescriptor(typeof(IQueryInterceptor<TQuery, TResult>), provider => new DelegatingQueryInterceptor<TQuery, TResult>(interceptAsync, provider), lifetime);
            ((CqrsConfigurationBuilder)configuration).Services.Add(service);
            return configuration;
        }

        private class DelegatingCommandHandler<TCommand> : ICommandHandler<TCommand>
            where TCommand : ICommand
        {
            private readonly Func<TCommand, CancellationToken, IServiceProvider, Task> handleAsync;
            private readonly IServiceProvider? serviceProvider;

            public DelegatingCommandHandler(Func<TCommand, CancellationToken, IServiceProvider, Task> handleAsync, IServiceProvider provider)
            {
                this.handleAsync = ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                serviceProvider = provider;
            }

            public DelegatingCommandHandler(Func<TCommand, CancellationToken, Task> handleAsync)
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                this.handleAsync = (command, cancellationToken, _) => handleAsync(command, cancellationToken);
            }

            public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
            {
                var task = handleAsync(command, cancellationToken, serviceProvider!);
                await task.ConfigureAwait(false);
            }
        }

        private class DelegatingCommandInterceptor<TCommand> : ICommandInterceptor<TCommand>
            where TCommand : ICommand
        {
            private readonly Func<TCommand, HandlerDelegate, CancellationToken, IServiceProvider, Task> interceptAsync;
            private readonly IServiceProvider? serviceProvider;

            public DelegatingCommandInterceptor(Func<TCommand, HandlerDelegate, CancellationToken, IServiceProvider, Task> interceptAsync, IServiceProvider provider)
            {
                this.interceptAsync = ArgumentNullExceptionHelper.ThrowIfNull(() => interceptAsync);
                serviceProvider = provider;
            }

            public DelegatingCommandInterceptor(Func<TCommand, HandlerDelegate, CancellationToken, Task> interceptAsync)
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => interceptAsync);
                this.interceptAsync = (command, next, cancellationToken, _) => interceptAsync(command, next, cancellationToken);
            }

            public async Task InterceptAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                var task = interceptAsync(command, next, cancellationToken, serviceProvider!);
                await task.ConfigureAwait(false);
            }
        }

        private class DelegatingEventHandler<TEvent> : IEventHandler<TEvent>
            where TEvent : IEvent
        {
            private readonly Func<TEvent, CancellationToken, IServiceProvider, Task> handleAsync;
            private readonly IServiceProvider? serviceProvider;

            public DelegatingEventHandler(Func<TEvent, CancellationToken, IServiceProvider, Task> handleAsync, IServiceProvider provider)
            {
                this.handleAsync = ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                serviceProvider = provider;
            }

            public DelegatingEventHandler(Func<TEvent, CancellationToken, Task> handleAsync)
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                this.handleAsync = (@event, cancellationToken, _) => handleAsync(@event, cancellationToken);
            }

            public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
            {
                var task = handleAsync(@event, cancellationToken, serviceProvider!);
                await task.ConfigureAwait(false);
            }
        }

        private class DelegatingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            private readonly Func<TQuery, CancellationToken, IServiceProvider, Task<TResult>> handleAsync;
            private readonly IServiceProvider? serviceProvider;

            public DelegatingQueryHandler(Func<TQuery, CancellationToken, IServiceProvider, Task<TResult>> handleAsync, IServiceProvider provider)
            {
                this.handleAsync = ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                serviceProvider = provider;
            }

            public DelegatingQueryHandler(Func<TQuery, CancellationToken, Task<TResult>> handleAsync)
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => handleAsync);
                this.handleAsync = (query, cancellationToken, _) => handleAsync(query, cancellationToken);
            }

            public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken)
            {
                var task = handleAsync(query, cancellationToken, serviceProvider!);
                return await task.ConfigureAwait(false);
            }
        }

        private class DelegatingQueryInterceptor<TQuery, TResult> : IQueryInterceptor<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            private readonly Func<TQuery, HandlerDelegate<TResult>, CancellationToken, IServiceProvider, Task<TResult>> interceptAsync;
            private readonly IServiceProvider? serviceProvider;

            public DelegatingQueryInterceptor(Func<TQuery, HandlerDelegate<TResult>, CancellationToken, IServiceProvider, Task<TResult>> interceptAsync, IServiceProvider provider)
            {
                this.interceptAsync = ArgumentNullExceptionHelper.ThrowIfNull(() => interceptAsync);
                serviceProvider = provider;
            }

            public DelegatingQueryInterceptor(Func<TQuery, HandlerDelegate<TResult>, CancellationToken, Task<TResult>> interceptAsync)
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => interceptAsync);
                this.interceptAsync = (query, next, cancellationToken, _) => interceptAsync(query, next, cancellationToken);
            }

            public async Task<TResult> InterceptAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
            {
                var task = interceptAsync(query, next, cancellationToken, serviceProvider!);
                return await task.ConfigureAwait(false);
            }
        }
    }
}
