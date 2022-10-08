﻿using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class DispatchingTests
    {
        private readonly Queue<Type> log = new();

        private ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.ConfigureCqrs()
                .AddDefaultDispatcher()
                .AddDefaultRegistry()
                .AddHandlersFromAssembly(Assembly.GetExecutingAssembly())
                .AddInterceptorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped(_ => log);
            return services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task DispatchAsync_GivenSampleEvent_DispatchesToAllHandlers()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IEventDispatcher dispatcher = provider.GetRequiredService<IEventDispatcher>();

            // Act
            await dispatcher.DispatchAsync(new SampleEvent());

            // Assert
            Assert.AreEqual(2, log.Count);
            Assert.IsTrue(log.Contains(typeof(SampleEventHandler)));
            Assert.IsTrue(log.Contains(typeof(GenericEventHandler<SampleEvent>)));
        }

        [TestMethod]
        public async Task CreateDelegate_GivenSampleCommand_CreatesDispatcherDelegate()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            ICommandDispatcher dispatcher = provider.GetRequiredService<ICommandDispatcher>();

            // Act
            var sampleCommandDelegate = dispatcher.CreateDelegate<SampleCommand>();
            await sampleCommandDelegate(new SampleCommand());

            // Assert
            Assert.IsInstanceOfType(sampleCommandDelegate, typeof(DispatcherDelegate<SampleCommand>));
            Assert.IsTrue(log.Any());
        }

        [TestMethod]
        public async Task CreateDelegate_GivenSampleEvent_CreatesDispatcherDelegate()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IEventDispatcher dispatcher = provider.GetRequiredService<IEventDispatcher>();

            // Act
            var sampleEventDelegate = dispatcher.CreateDelegate<SampleEvent>();
            await sampleEventDelegate(new SampleEvent());

            // Assert
            Assert.IsInstanceOfType(sampleEventDelegate, typeof(DispatcherDelegate<SampleEvent>));
            Assert.IsTrue(log.Any());
        }

        [TestMethod]
        public async Task CreateDelegate_GivenSampleQuery_CreatesDispatcherDelegate()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IQueryDispatcher dispatcher = provider.GetRequiredService<IQueryDispatcher>();

            // Act
            var sampleQueryDelegate = dispatcher.CreateDelegate<SampleQuery, SampleQueryResult>();
            await sampleQueryDelegate(new SampleQuery());

            // Assert
            Assert.IsInstanceOfType(sampleQueryDelegate, typeof(DispatcherDelegate<SampleQuery, SampleQueryResult>));
            Assert.IsTrue(log.Any());
        }
    }
}
