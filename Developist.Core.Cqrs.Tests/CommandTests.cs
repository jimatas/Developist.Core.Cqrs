using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class CommandTests
    {
        #region Fixture
        public class BaseCommand : ICommand { }
        public class DerivedCommand : BaseCommand { }

        public class BaseCommandHandler : ICommandHandler<BaseCommand>
        {
            private readonly Queue<Type> log;
            public BaseCommandHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(BaseCommand command, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }

        public class DerivedCommandHandler : ICommandHandler<DerivedCommand>
        {
            private readonly Queue<Type> log;
            public DerivedCommandHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(DerivedCommand command, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }
        #endregion

        #region Setup
        private readonly Queue<Type> log = new();

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            var services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }

        private ServiceProvider CreateServiceProviderWithDefaultConfiguration()
        {
            return ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDefaultDispatcher();
                    builder.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                    builder.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
                });
                services.AddScoped(_ => log);
            });
        }
        #endregion

        [TestMethod]
        public async Task DispatchAsync_GivenNull_ThrowsNullArgumentException()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            Task action() => commandDispatcher.DispatchAsync<ICommand>(null!);

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
            Assert.AreEqual("Value cannot be null. (Parameter 'command')", exception.Message);
        }

        [TestMethod]
        public async Task DynamicDispatchAsync_GivenNull_ThrowsNullArgumentException()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

            // Act
            Task action() => commandDispatcher.DispatchAsync(null!);

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
            Assert.AreEqual("Value cannot be null. (Parameter 'command')", exception.Message);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenBaseCommand_DispatchesToBaseCommandHandler()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new BaseCommand());

            // Assert
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(BaseCommandHandler), log.Single());
        }

        [TestMethod]
        public async Task DispatchAsync_GivenBaseCommandAsICommand_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            Task action() => commandDispatcher.DispatchAsync((ICommand)new BaseCommand());

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
            Assert.AreEqual($"No handler found for command with type '{typeof(ICommand)}'.", exception.Message);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenDerivedCommand_DispatchesToDerivedCommandHandler()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new DerivedCommand());

            // Assert
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(DerivedCommandHandler), log.Single());
        }

        [TestMethod]
        public async Task DispatchAsync_GivenDerivedCommandAsICommand_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            Task action() => commandDispatcher.DispatchAsync((ICommand)new DerivedCommand());

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
            Assert.AreEqual($"No handler found for command with type '{typeof(ICommand)}'.", exception.Message);
        }

        [TestMethod]
        public async Task DynamicDispatchAsync_GivenBaseCommand_DispatchesToBaseCommandHandler()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new BaseCommand());

            // Assert
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(BaseCommandHandler), log.Single());
        }

        [TestMethod]
        public async Task DynamicDispatchAsync_GivenDerivedCommand_DispatchesToDerivedCommandHandler()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new DerivedCommand());

            // Assert
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(DerivedCommandHandler), log.Single());
        }
    }
}
