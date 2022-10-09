using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class DelegatingHandlerTests
    {
        [TestMethod]
        public async Task AddQueryHandler_ByDefault_RegistersDelegate()
        {
            UserRepository userRepository = new();
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddQueryHandler<GetUserQuery, User?>((query, token) =>
                    {
                        var user = userRepository.FirstOrDefault(
                            u => u.UserName.Equals(query.UserName, query.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

                        return Task.FromResult(user);
                    });
                });
            });

            var queryHandler = provider.GetService<IQueryHandler<GetUserQuery, User?>>();
            Assert.IsNotNull(queryHandler);

            var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();

            var queryResult = await queryDispatcher.DispatchAsync(new GetUserQuery("WelshD"));
            Assert.IsNotNull(queryResult);
        }

        [TestMethod]
        public async Task AddQueryHandler_WithServiceProvider_RegistersDelegate()
        {
            bool serviceProviderSupplied = false;
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddQueryHandler<GetUserQuery, User?>((query, provider, token) =>
                    {
                        serviceProviderSupplied = provider is not null;
                        return Task.FromResult<User?>(null);
                    });
                });
            });

            var queryHandler = provider.GetService<IQueryHandler<GetUserQuery, User?>>();
            Assert.IsNotNull(queryHandler);

            await queryHandler.HandleAsync(new GetUserQuery("Unknown"), CancellationToken.None);
            Assert.IsTrue(serviceProviderSupplied);
        }

        [TestMethod]
        public async Task AddQueryInterceptor_ByDefault_RegistersDelegate()
        {
            UserRepository userRepository = new();
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddQueryHandler<GetUserQuery, User?>((query, token) =>
                    {
                        var user = userRepository.FirstOrDefault(
                            u => u.UserName.Equals(query.UserName, query.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));

                        return Task.FromResult(user);
                    });
                    builder.AddQueryInterceptor<GetUserQuery, User?>((query, next, token) =>
                    {
                        query.IsCaseSensitive = false;
                        return next();
                    });
                });
            });

            var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
            var queryResult = await queryDispatcher.DispatchAsync(new GetUserQuery("welshd"));
            Assert.IsNotNull(queryResult);
        }

        [TestMethod]
        public async Task AddQueryInterceptor_WithServiceProvider_RegistersDelegate()
        {
            bool serviceProviderSupplied = false;
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddQueryHandler<GetUserQuery, User?>((query, token) => Task.FromResult<User?>(null));
                    builder.AddQueryInterceptor((GetUserQuery query, HandlerDelegate<User?> next, IServiceProvider provider, CancellationToken token) =>
                    {
                        serviceProviderSupplied = provider is not null;
                        return next();
                    });
                });
            });

            var queryInterceptor = provider.GetService<IQueryInterceptor<GetUserQuery, User?>>();
            Assert.IsNotNull(queryInterceptor);

            await queryInterceptor.InterceptAsync(new GetUserQuery("Unknown"), () => Task.FromResult((User?)null), CancellationToken.None);
            Assert.IsTrue(serviceProviderSupplied);
        }

        [TestMethod]
        public async Task AddCommandHandler_ByDefault_RegistersDelegate()
        {
            UserRepository userRepository = new();
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddCommandHandler<AddUserCommand>((command, token) =>
                    {
                        userRepository.Add(command.UserName, command.DisplayName);
                        return Task.CompletedTask;
                    });
                });
            });

            var commandHandler = provider.GetService<ICommandHandler<AddUserCommand>>();
            Assert.IsNotNull(commandHandler);

            var commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();

            await commandDispatcher.DispatchAsync(new AddUserCommand("SmithJ", "John", "Smith"));
            Assert.IsNotNull(userRepository.FirstOrDefault(u => u.UserName.Equals("SmithJ")));
        }

        [TestMethod]
        public async Task AddCommandHandler_WithServiceProvider_RegistersDelegate()
        {
            bool serviceProviderSupplied = false;
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddCommandHandler<AddUserCommand>((command, provider, token) =>
                    {
                        serviceProviderSupplied = provider is not null;
                        return Task.CompletedTask;
                    });
                });
            });

            var commandHandler = provider.GetService<ICommandHandler<AddUserCommand>>();
            Assert.IsNotNull(commandHandler);

            await commandHandler.HandleAsync(new AddUserCommand("SmithJ", "John", "Smith"), CancellationToken.None);
            Assert.IsTrue(serviceProviderSupplied);
        }

        [TestMethod]
        public async Task AddCommandInterceptor_ByDefault_RegistersDelegate()
        {
            UserRepository userRepository = new();
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddCommandHandler<AddUserCommand>((command, token) =>
                    {
                        userRepository.Add(command.UserName, command.DisplayName);
                        return Task.CompletedTask;
                    });
                    builder.AddCommandInterceptor(async (AddUserCommand command, HandlerDelegate next, CancellationToken token) =>
                    {
                        command.DisplayName = $"{command.FirstName} {command.LastName}";
                        await next();
                    });
                });
            });

            var commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();
            await commandDispatcher.DispatchAsync(new AddUserCommand("SmithJ", "John", "Smith"));

            var user = userRepository.FirstOrDefault(u => u.UserName.Equals("SmithJ"))!;
            Assert.AreEqual("John Smith", user.DisplayName);
        }

        [TestMethod]
        public async Task AddCommandInterceptor_WithServiceProvider_RegistersDelegate()
        {
            bool serviceProviderSupplied = false;
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddCommandHandler<AddUserCommand>((command, token) => Task.CompletedTask);
                    builder.AddCommandInterceptor((AddUserCommand command, HandlerDelegate next, IServiceProvider provider, CancellationToken token) =>
                    {
                        serviceProviderSupplied = provider is not null;
                        return next();
                    });
                });
            });

            var commandInterceptor = provider.GetService<ICommandInterceptor<AddUserCommand>>();
            Assert.IsNotNull(commandInterceptor);

            await commandInterceptor.InterceptAsync(new AddUserCommand("SmithJ", "John", "Smith"), () => Task.CompletedTask, CancellationToken.None);
            Assert.IsTrue(serviceProviderSupplied);
        }

        [TestMethod]
        public void AddEventHandler_ByDefault_RegistersDelegate()
        {
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddEventHandler<UserLoggedIn>((@event, token) =>
                    {
                        return Task.CompletedTask;
                    });
                });
            });

            var eventHandlers = provider.GetServices<IEventHandler<UserLoggedIn>>();
            Assert.IsTrue(eventHandlers.Any());
        }

        [TestMethod]
        public async Task AddEventHandler_WithServiceProvider_RegistersDelegate()
        {
            User? loggedInUser = null;
            using var provider = ConfigureServiceProvider(services =>
            {
                services.AddSingleton<UserRepository>();
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddDefaultRegistry();
                    builder.AddEventHandler<UserLoggedIn>((@event, provider, token) =>
                    {
                        var repository = provider.GetRequiredService<UserRepository>();
                        loggedInUser = repository.FirstOrDefault(u => u.UserName.Equals(@event.UserName));
                        return Task.CompletedTask;
                    });
                });
            });

            var eventDispatcher = provider.GetRequiredService<IEventDispatcher>();
            await eventDispatcher.DispatchAsync(new UserLoggedIn("MarinH"));

            Assert.IsNotNull(loggedInUser);
            Assert.AreEqual("MarinH", loggedInUser.UserName);
        }

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            var services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }
    }
}
