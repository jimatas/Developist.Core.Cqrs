using Developist.Core.Cqrs.Tests.Fixture.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class EventTests : TestClassBase
{
    [TestMethod]
    public async Task DispatchAsync_NullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
            });
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
        IEvent @event = null!;

        // Act
        var action = () => dispatcher.DispatchAsync(@event);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(@event), exception.ParamName);
    }

    [TestMethod]
    public async Task DispatchAsync_SampleEvent_DispatchesToSampleEventHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddEventHandler<SampleEvent, SampleEventHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleEvent());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<SampleEventHandler>(log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_SampleEventAsIEvent_DoesNotDispatch()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddEventHandler<SampleEvent, SampleEventHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await dispatcher.DispatchAsync((IEvent)new SampleEvent());

        // Assert
        Assert.IsFalse(log.Any());
    }

    [TestMethod]
    public async Task DispatchAsync_BaseEvent_DispatchesToBaseEventHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddEventHandler<BaseEvent, BaseEventHandler>();
                cfg.AddEventHandler<DerivedEvent, DerivedEventHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new BaseEvent());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<BaseEventHandler>(log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_DerivedEvent_DispatchesToDerivedEventHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddEventHandler<BaseEvent, BaseEventHandler>();
                cfg.AddEventHandler<DerivedEvent, DerivedEventHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new DerivedEvent());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<DerivedEventHandler>(log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_FaultingEventHandler_StillDispatchesToOthers()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddEventHandler<SampleEvent, FaultingSampleEventHandler>();
                cfg.AddEventHandler<SampleEvent, SampleEventHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

        // Act
        var action = () => dispatcher.DispatchAsync(new SampleEvent());

        // Assert
        await Assert.ThrowsExceptionAsync<AggregateException>(action);
        Assert.IsTrue(log.Any());
    }
}
