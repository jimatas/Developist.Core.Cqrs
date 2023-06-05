using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class CommandTests
{
    private readonly Queue<Type> _log = new();

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
    public async Task DispatchAsync_GivenFaultingCommand_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CommandDispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
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

    private ServiceProvider CreateServiceProviderWithDefaultConfiguration()
    {
        return ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                builder.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
            });
            services.AddScoped(_ => _log);
        });
    }
}
