using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class EventTests
    {
        #region Fixture
        public class BaseEvent : IEvent { }
        public class DerivedEvent : BaseEvent { }

        public class BaseEventHandler : IEventHandler<BaseEvent>
        {
            private readonly Queue<Type> log;
            public BaseEventHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(BaseEvent @event, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }

        public class DerivedEventHandler : IEventHandler<DerivedEvent>
        {
            private readonly Queue<Type> log;
            public DerivedEventHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(DerivedEvent @event, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }

        public class SampleEvent : IEvent { }

        public class SampleEventHandler : IEventHandler<SampleEvent>
        {
            private readonly Queue<Type> log;
            public SampleEventHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }

        public class AnotherSampleEventHandler : IEventHandler<SampleEvent>
        {
            private readonly Queue<Type> log;
            public AnotherSampleEventHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
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
                    builder.AddDispatcher();
                    builder.AddDynamicDispatcher();
                    builder.AddEventHandler<BaseEvent, BaseEventHandler>();
                    builder.AddEventHandler<DerivedEvent, DerivedEventHandler>();
                    builder.AddEventHandler<SampleEvent, SampleEventHandler>();
                    builder.AddEventHandler<SampleEvent, AnotherSampleEventHandler>();
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
            var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
            
            // Act
            Task action() => eventDispatcher.DispatchAsync<SampleEvent>(null!);

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
            Task action() => eventDispatcher.DispatchAsync(null!);

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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(BaseEventHandler), log.Single());
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
            Assert.IsFalse(log.Any());
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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(DerivedEventHandler), log.Single());
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
            Assert.IsFalse(log.Any());
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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(BaseEventHandler), log.Single());
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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(DerivedEventHandler), log.Single());
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
            Assert.AreEqual(2, log.Count);
            Assert.IsTrue(log.Contains(typeof(SampleEventHandler)));
            Assert.IsTrue(log.Contains(typeof(AnotherSampleEventHandler)));
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
            Assert.IsFalse(log.Any());
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
            Assert.AreEqual(2, log.Count);
            Assert.IsTrue(log.Contains(typeof(SampleEventHandler)));
            Assert.IsTrue(log.Contains(typeof(AnotherSampleEventHandler)));
        }
    }
}
