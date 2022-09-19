using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
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
    }
}
