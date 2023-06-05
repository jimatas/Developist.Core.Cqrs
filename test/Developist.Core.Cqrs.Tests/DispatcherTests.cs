using Developist.Core.Cqrs.Tests.Fixture;
using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Developist.Core.Cqrs.Tests.Fixture.Events;
using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class DispatcherTests
{
    private readonly Queue<Type> _log = new();

    [TestMethod]
    public void InitializeDispatcher_GivenNullCommandDispatcher_ThrowsArgumentNullException()
    {
        // Arrange
        ICommandDispatcher? commandDispatcher = null;
        IEventDispatcher eventDispatcher = new EventDispatcher(new EmptyHandlerRegistry());
        IQueryDispatcher queryDispatcher = new QueryDispatcher(new EmptyHandlerRegistry());

        // Act
        var action = () => new Dispatcher(commandDispatcher, eventDispatcher, queryDispatcher);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(commandDispatcher), exception.ParamName);
    }

    [TestMethod]
    public void InitializeDispatcher_GivenNullEventDispatcher_ThrowsArgumentNullException()
    {
        // Arrange
        ICommandDispatcher commandDispatcher = new CommandDispatcher(new EmptyHandlerRegistry());
        IEventDispatcher? eventDispatcher = null;
        IQueryDispatcher queryDispatcher = new QueryDispatcher(new EmptyHandlerRegistry());

        // Act
        var action = () => new Dispatcher(commandDispatcher, eventDispatcher, queryDispatcher);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(eventDispatcher), exception.ParamName);
    }

    [TestMethod]
    public void InitializeDispatcher_GivenNullQueryDispatcher_ThrowsArgumentNullException()
    {
        // Arrange
        ICommandDispatcher commandDispatcher = new CommandDispatcher(new EmptyHandlerRegistry());
        IEventDispatcher eventDispatcher = new EventDispatcher(new EmptyHandlerRegistry());
        IQueryDispatcher? queryDispatcher = null;

        // Act
        var action = () => new Dispatcher(commandDispatcher, eventDispatcher, queryDispatcher);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(queryDispatcher), exception.ParamName);
    }

    [TestMethod]
    public void InitializeCommandDispatcher_GivenNullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry? registry = null;

        // Act
        var action = () => new CommandDispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }

    [TestMethod]
    public void InitializeEventDispatcher_GivenNullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry? registry = null;

        // Act
        var action = () => new EventDispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }

    [TestMethod]
    public void InitializeQueryDispatcher_GivenNullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry? registry = null;

        // Act
        var action = () => new QueryDispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }

    [TestMethod]
    public async Task DispatchAsync_GivenCommand_DelegatesToCommandDispatcher()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddCommandHandler<SampleCommand, SampleCommandHandler>();
            });
            services.AddScoped(_ => _log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.AreEqual(typeof(SampleCommandHandler), _log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenEvent_DelegatesToEventDispatcher()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddEventHandler<SampleEvent, SampleEventHandler>();
            });
            services.AddScoped(_ => _log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleEvent());

        // Assert
        Assert.AreEqual(typeof(SampleEventHandler), _log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenQuery_DelegatesToQueryDispatcher()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddQueryHandler<SampleQuery, SampleQueryResult, SampleQueryHandler>();
            });
            services.AddScoped(_ => _log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        // Act
        var result = await dispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.AreEqual(typeof(SampleQueryHandler), _log.Dequeue());
        Assert.IsInstanceOfType(result, typeof(SampleQueryResult));
    }
}
