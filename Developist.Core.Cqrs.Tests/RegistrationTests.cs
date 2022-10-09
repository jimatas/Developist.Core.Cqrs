using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class RegistrationTests
    {
        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddCqrs(builder => 
                builder.AddDefaultDispatcher()
                    .AddDefaultRegistry()
                    .AddHandlersFromAssembly(Assembly.GetExecutingAssembly())
                    .AddInterceptorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped(_ => new Queue<Type>());
            return services.BuildServiceProvider();
        }

        [TestMethod]
        public void AddDefaultDispatcher_RegistersAllDispatcherInterfaces()
        {
            // Arrange
            using var provider = CreateServiceProvider();

            // Act
            IDispatcher? dispatcher = provider.GetService<IDispatcher>();
            ICommandDispatcher? commandDispatcher = provider.GetService<ICommandDispatcher>();
            IQueryDispatcher? queryDispatcher = provider.GetService<IQueryDispatcher>();
            IEventDispatcher? eventDispatcher = provider.GetService<IEventDispatcher>();

            // Assert
            Assert.IsNotNull(dispatcher);
            Assert.IsNotNull(commandDispatcher);
            Assert.IsNotNull(queryDispatcher);
            Assert.IsNotNull(eventDispatcher);
        }

        [TestMethod]
        public void AddDefaultDispatcher_RegistersSingleDispatcherInstance()
        {
            // Arrange
            using var provider = CreateServiceProvider();

            // Act
            IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();
            ICommandDispatcher commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();
            IQueryDispatcher queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
            IEventDispatcher eventDispatcher = provider.GetRequiredService<IEventDispatcher>();

            // Assert
            Assert.AreEqual(dispatcher, commandDispatcher);
            Assert.AreEqual(dispatcher, queryDispatcher);
            Assert.AreEqual(dispatcher, eventDispatcher);
        }

        [TestMethod]
        public void AddHandlersFromAssembly_RegistersHandlers()
        {
            // Arrange
            using var provider = CreateServiceProvider();

            // Act
            var sampleCommandHandler = provider.GetService<ICommandHandler<SampleCommand>>();
            var sampleQueryHandler = provider.GetService<IQueryHandler<SampleQuery, SampleQueryResult>>();
            var sampleEventHandlers = provider.GetServices<IEventHandler<SampleEvent>>();

            // Assert
            Assert.IsNotNull(sampleCommandHandler);
            Assert.IsNotNull(sampleQueryHandler);
            Assert.IsTrue(sampleEventHandlers.Any());
        }

        [TestMethod]
        public void AddInterceptorsFromAssembly_RegistersInterceptors()
        {
            // Arrange
            using var provider = CreateServiceProvider();

            // Act
            var sampleCommandInterceptors = provider.GetServices<ICommandInterceptor<SampleCommand>>();
            var sampleQueryInterceptors = provider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>();

            // Assert
            Assert.IsTrue(sampleCommandInterceptors.Any());
            Assert.IsTrue(sampleQueryInterceptors.Any());
        }

        [TestMethod]
        public void GetCommandHandler_CommandWithoutHandler_ThrowsInvalidOperationException()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            var handlerRegistry = provider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetCommandHandler(typeof(CommandWithoutHandler));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"No handler found for command with type {typeof(CommandWithoutHandler)}.", exception.Message);
        }

        [TestMethod]
        public void GetCommandHandler_CommandWithMultipleHandlers_ThrowsInvalidOperationException()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            var handlerRegistry = provider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetCommandHandler(typeof(CommandWithMultipleHandlers));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"More than one handler found for command with type {typeof(CommandWithMultipleHandlers)}.", exception.Message);
        }
    }
}
