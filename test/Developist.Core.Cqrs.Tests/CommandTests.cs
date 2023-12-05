using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class CommandTests : TestClassBase
{
    [TestMethod]
    public async Task DispatchAsync_NullCommand_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
            });
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
        ICommand command = null!;

        // Act
        var action = () => dispatcher.DispatchAsync(command);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(command), exception.ParamName);
    }

    [TestMethod]
    public async Task DispatchAsync_SampleCommand_DispatchesToSampleCommandHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<SampleCommand, SampleCommandHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.IsInstanceOfType<SampleCommandHandler>(log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_BaseCommand_DispatchesToBaseCommandHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                cfg.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new BaseCommand());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<BaseCommandHandler>(log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_BaseCommandAsICommand_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                cfg.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
            });
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => dispatcher.DispatchAsync((ICommand)new BaseCommand());

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for command '{typeof(ICommand)}'.", exception.Message);
    }

    [TestMethod]
    public async Task DispatchAsync_DerivedCommand_DispatchesToDerivedCommandHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<BaseCommand, BaseCommandHandler>();
                cfg.AddCommandHandler<DerivedCommand, DerivedCommandHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new DerivedCommand());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<DerivedCommandHandler>(log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_FaultingCommandHandler_LogsExceptionAndThrows()
    {
        // Arrange
        var log = new Queue<object>();

        var logger = new Mock<ILogger<DefaultDispatcher>>();
        logger.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<SampleCommand, FaultingSampleCommandHandler>();
            });
            services.AddScoped(_ => log);
            services.AddScoped(_ => logger.Object);
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => dispatcher.DispatchAsync(new SampleCommand());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        logger.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during command dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [TestMethod]
    public async Task DispatchAsync_SampleCommand_RunsInterceptorsInExpectedOrder()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddCommandHandler<SampleCommand, SampleCommandHandler>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithHighestPriority>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPriority>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPlusOnePriority>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithImplicitNormalPriority>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryLowPriority>();
                cfg.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryHighPriority>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.IsInstanceOfType<SampleCommandInterceptorWithHighestPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandInterceptorWithVeryHighPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandInterceptorWithImplicitNormalPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandInterceptorWithVeryLowPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandInterceptorWithLowestPlusOnePriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandInterceptorWithLowestPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleCommandHandler>(log.Dequeue());
    }
}
