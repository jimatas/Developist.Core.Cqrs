using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class EventTests
{
    #region Fixture
    public record BaseEvent : IEvent;
    public record DerivedEvent : BaseEvent;

    public class BaseEventHandler : IEventHandler<BaseEvent>
    {
        private readonly Queue<Type> _log;
        public BaseEventHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class DerivedEventHandler : IEventHandler<DerivedEvent>
    {
        private readonly Queue<Type> _log;
        public DerivedEventHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(DerivedEvent @event, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public record SampleEvent : IEvent;

    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        private readonly Queue<Type> _log;
        public SampleEventHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class AnotherSampleEventHandler : IEventHandler<SampleEvent>
    {
        private readonly Queue<Type> _log;
        public AnotherSampleEventHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class FaultingEventHandler : IEventHandler<SampleEvent>
    {
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
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
                builder.AddEventHandler<BaseEvent, BaseEventHandler>();
                builder.AddEventHandler<DerivedEvent, DerivedEventHandler>();
                builder.AddEventHandler<SampleEvent, SampleEventHandler>();
                builder.AddEventHandler<SampleEvent, AnotherSampleEventHandler>();
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
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        var action = () => eventDispatcher.DispatchAsync<SampleEvent>(null!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual("Value cannot be null. (Parameter 'event')", exception.Message);
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenNull_ThrowsNullArgumentException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();

        // Act
        var action = () => eventDispatcher.DispatchAsync(null!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual("Value cannot be null. (Parameter 'event')", exception.Message);
    }

    [TestMethod]
    public async Task DispatchAsync_GivenBaseEvent_DispatchesToBaseEventHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new BaseEvent());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseEventHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenBaseEventAsIEvent_DoesNotDispatch()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync((IEvent)new BaseEvent());

        // Assert
        Assert.IsFalse(_log.Any());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenDerivedEvent_DispatchesToDerivedEventHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new DerivedEvent());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedEventHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenDerivedEventAsIEvent_DoesNotDispatch()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync((IEvent)new DerivedEvent());

        // Assert
        Assert.IsFalse(_log.Any());
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenBaseEvent_DispatchesToBaseEventHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new BaseEvent());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseEventHandler), _log.Single());
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenDerivedEvent_DispatchesToDerivedEventHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new DerivedEvent());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedEventHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenSampleEvent_DispatchesToAllAppropriateEventHandlers()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new SampleEvent());

        // Assert
        Assert.AreEqual(2, _log.Count);
        Assert.IsTrue(_log.Contains(typeof(SampleEventHandler)));
        Assert.IsTrue(_log.Contains(typeof(AnotherSampleEventHandler)));
    }

    [TestMethod]
    public async Task DispatchAsync_GivenSampleEventAsIEvent_DoesNotDispatch()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync((IEvent)new SampleEvent());

        // Assert
        Assert.IsFalse(_log.Any());
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenSampleEvent_DispatchesToAllAppropriateEventHandlers()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();

        // Act
        await eventDispatcher.DispatchAsync(new SampleEvent());

        // Assert
        Assert.AreEqual(2, _log.Count);
        Assert.IsTrue(_log.Contains(typeof(SampleEventHandler)));
        Assert.IsTrue(_log.Contains(typeof(AnotherSampleEventHandler)));
    }

    [TestMethod]
    public async Task DispatchAsync_GivenFaultingEvent_DispatchesToAll()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatcher();
                builder.AddEventHandler<SampleEvent, SampleEventHandler>();
                builder.AddEventHandler<SampleEvent, FaultingEventHandler>();
                builder.AddEventHandler<SampleEvent, AnotherSampleEventHandler>();
            });
            services.AddScoped(_ => _log);
        });

        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        var action = () => eventDispatcher.DispatchAsync(new SampleEvent());

        // Assert
        await Assert.ThrowsExceptionAsync<AggregateException>(action);
        Assert.AreEqual(2, _log.Count);
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenFaultingEvent_DispatchesToAll()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDynamicDispatcher();
                builder.AddEventHandler<SampleEvent, SampleEventHandler>();
                builder.AddEventHandler<SampleEvent, FaultingEventHandler>();
                builder.AddEventHandler<SampleEvent, AnotherSampleEventHandler>();
            });
            services.AddScoped(_ => _log);
        });

        var eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();

        // Act
        var action = () => eventDispatcher.DispatchAsync(new SampleEvent());

        // Assert
        await Assert.ThrowsExceptionAsync<AggregateException>(action);
        Assert.AreEqual(2, _log.Count);
    }
}
