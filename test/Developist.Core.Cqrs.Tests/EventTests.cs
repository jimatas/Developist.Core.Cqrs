using Developist.Core.Cqrs.Tests.Fixture.Events;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class EventTests
{
    private readonly Queue<Type> _log = new();

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
    public async Task DispatchAsync_GivenFaultingEvent_DispatchesToAll()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
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

    private ServiceProvider CreateServiceProviderWithDefaultConfiguration()
    {
        return ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddEventHandler<BaseEvent, BaseEventHandler>();
                builder.AddEventHandler<DerivedEvent, DerivedEventHandler>();
                builder.AddEventHandler<SampleEvent, SampleEventHandler>();
                builder.AddEventHandler<SampleEvent, AnotherSampleEventHandler>();
            });
            services.AddScoped(_ => _log);
        });
    }
}
