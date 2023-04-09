using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class CommandTests
{
    #region Fixture
    public record BaseCommand : ICommand;
    public record DerivedCommand : BaseCommand;
    public record FaultingCommand : ICommand;

    public class BaseCommandHandler : ICommandHandler<BaseCommand>
    {
        private readonly Queue<Type> _log;
        public BaseCommandHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(BaseCommand command, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class DerivedCommandHandler : ICommandHandler<DerivedCommand>
    {
        private readonly Queue<Type> _log;
        public DerivedCommandHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(DerivedCommand command, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class FaultingCommandHandler : ICommandHandler<FaultingCommand>
    {
        public Task HandleAsync(FaultingCommand command, CancellationToken cancellationToken)
        {
            throw new ApplicationException("There was an error.");
        }
    }
    #endregion

    #region Setup
    private readonly Queue<Type> _log = new();

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
                builder.AddDispatcher();
                builder.AddDynamicDispatcher();
                builder.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                builder.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
            });
            services.AddScoped(_ => _log);
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
        var action = () => commandDispatcher.DispatchAsync<ICommand>(null!);

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
        var action = () => commandDispatcher.DispatchAsync(null!);

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
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseCommandHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenBaseCommandAsICommand_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync((ICommand)new BaseCommand());

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
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseCommandHandler), _log.Single());
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
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedCommandHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenDerivedCommandAsICommand_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync((ICommand)new DerivedCommand());

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for command with type '{typeof(ICommand)}'.", exception.Message);
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
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedCommandHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenFaultingCommand_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Dispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatcher();
                builder.AddCommandHandler<FaultingCommand, FaultingCommandHandler>();
            });
            services.AddScoped(_ => _log);
            services.AddScoped(_ => loggerMock.Object);
        });

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync(new FaultingCommand());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        loggerMock.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during command dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenFaultingCommand_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DynamicDispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDynamicDispatcher();
                builder.AddCommandHandler<FaultingCommand, FaultingCommandHandler>();
            });
            services.AddScoped(_ => _log);
            services.AddScoped(_ => loggerMock.Object);
        });

        var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync(new FaultingCommand());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        loggerMock.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during command dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }
}
