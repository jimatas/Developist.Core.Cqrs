using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;

using static Developist.Core.Cqrs.Tests.CommandInterceptorTests;
using static Developist.Core.Cqrs.Tests.EventTests;
using static Developist.Core.Cqrs.Tests.QueryInterceptorTests;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class DispatcherTests
    {
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
                    builder.AddDefaultDispatcher();
                    builder.AddHandlersFromAssembly(GetType().Assembly);
                });
                services.AddScoped(_ => log);
            });
        }
        #endregion

        [TestMethod]
        public async Task CreateDelegate_GivenSampleCommand_CreatesDispatcherDelegate()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            var sampleCommandDelegate = commandDispatcher.CreateDelegate<SampleCommand>();
            await sampleCommandDelegate(new SampleCommand());

            // Assert
            Assert.IsInstanceOfType(sampleCommandDelegate, typeof(DispatcherDelegate<SampleCommand>));
            Assert.IsTrue(log.Any());
        }

        [TestMethod]
        public async Task CreateDelegate_GivenSampleEvent_CreatesDispatcherDelegate()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            // Act
            var sampleEventDelegate = eventDispatcher.CreateDelegate<SampleEvent>();
            await sampleEventDelegate(new SampleEvent());

            // Assert
            Assert.IsInstanceOfType(sampleEventDelegate, typeof(DispatcherDelegate<SampleEvent>));
            Assert.IsTrue(log.Any());
        }

        [TestMethod]
        public async Task CreateDelegate_GivenSampleQuery_CreatesDispatcherDelegate()
        {
            // Arrange
            using var serivceProvider = CreateServiceProviderWithDefaultConfiguration();
            var queryDispatcher = serivceProvider.GetRequiredService<IQueryDispatcher>();

            // Act
            var sampleQueryDelegate = queryDispatcher.CreateDelegate<SampleQuery, SampleQueryResult>();
            await sampleQueryDelegate(new SampleQuery());

            // Assert
            Assert.IsInstanceOfType(sampleQueryDelegate, typeof(DispatcherDelegate<SampleQuery, SampleQueryResult>));
            Assert.IsTrue(log.Any());
        }
    }
}
