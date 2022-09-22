using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class ArgumentNullTests
    {
        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.ConfigureCqrs()
                .AddDefaultDispatcher()
                .AddDefaultRegistry();

            return services.BuildServiceProvider();
        }

        [TestMethod]
        public void InitializeDefaultRegistry_GivenNullServiceProvider_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            static void action() => _ = new DefaultRegistry(provider: null!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void InitializeDefaultDispatcher_GivenNullHandlerRegistry_ThrowsArgumentNullException()
        {
            // Arrange
            IInterceptorRegistry interceptorRegistry = new DefaultRegistry(CreateServiceProvider());

            // Act
            void action() => _ = new DefaultDispatcher(handlerRegistry: null!, interceptorRegistry);

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual("handlerRegistry", exception.ParamName);
        }

        [TestMethod]
        public void InitializeDefaultDispatcher_GivenNullInterceptorRegistry_ThrowsArgumentNullException()
        {
            // Arrange
            IHandlerRegistry handlerRegistry = new DefaultRegistry(CreateServiceProvider());

            // Act
            void action() => _ = new DefaultDispatcher(handlerRegistry, interceptorRegistry: null!);

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual("interceptorRegistry", exception.ParamName);
        }

        [TestMethod]
        public void AddHandlersFromAssembly_GivenNullAssembly_ThrowsArgumentNullException()
        {
            // Arrange
            CqrsConfigurationBuilder builder = (CqrsConfigurationBuilder)new ServiceCollection().ConfigureCqrs();
            
            // Act
            void action() => ((IHandlerConfiguration)builder).AddHandlersFromAssembly(null!);

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual("assembly", exception.ParamName);
        }

        [TestMethod]
        public void AddInterceptorsFromAssembly_GivenNullAssembly_ThrowsArgumentNullException()
        {
            // Arrange
            CqrsConfigurationBuilder builder = (CqrsConfigurationBuilder)new ServiceCollection().ConfigureCqrs();

            // Act
            void action() => ((IInterceptorConfiguration)builder).AddInterceptorsFromAssembly(null!);

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual("assembly", exception.ParamName);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenNullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            ICommandDispatcher dispatcher = provider.GetRequiredService<ICommandDispatcher>();

            // Act
            async Task action() => await dispatcher.DispatchAsync((SampleCommand?)null!);

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
            Assert.AreEqual("command", exception.ParamName);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenNullQuery_ThrowsArgumentNullException()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IQueryDispatcher dispatcher = provider.GetRequiredService<IQueryDispatcher>();

            // Act
            async Task action() => await dispatcher.DispatchAsync((SampleQuery)null!);

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
            Assert.AreEqual("query", exception.ParamName);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenNullEvent_ThrowsArgumentNullException()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IEventDispatcher dispatcher = provider.GetRequiredService<IEventDispatcher>();

            // Act
            async Task action() => await dispatcher.DispatchAsync((SampleEvent)null!);

            // Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
            Assert.AreEqual("event", exception.ParamName);
        }
    }
}
