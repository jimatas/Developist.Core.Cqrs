﻿using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Infrastructure;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;

using static Developist.Core.Cqrs.Tests.CommandInterceptorTests;
using static Developist.Core.Cqrs.Tests.EventTests;
using static Developist.Core.Cqrs.Tests.QueryInterceptorTests;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class RegistrationTests
    {
        #region Fixture
        public record CommandWithoutHandler : ICommand { }
        public record CommandWithMultipleHandlers : ICommand { }

        public class CommandWithMultipleHandlersFirstHandler : ICommandHandler<CommandWithMultipleHandlers>
        {
            public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class CommandWithMultipleHandlersSecondHandler : ICommandHandler<CommandWithMultipleHandlers>
        {
            public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public record QueryWithoutHandler : IQuery<SampleQueryResult> { }
        public record QueryWithMultipleHandlers : IQuery<SampleQueryResult> { }

        public class QueryWithMultipleHandlersFirstHandler : IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>
        {
            public Task<SampleQueryResult> HandleAsync(QueryWithMultipleHandlers query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class QueryWithMultipleHandlersSecondHandler : IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>
        {
            public Task<SampleQueryResult> HandleAsync(QueryWithMultipleHandlers query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Setup
        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            var services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }
        #endregion

        [TestMethod]
        public void AddDispatcher_RegistersAllDispatcherInterfaces()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                });
            });

            // Act
            IDispatcher? dispatcher = serviceProvider.GetService<IDispatcher>();
            ICommandDispatcher? commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
            IQueryDispatcher? queryDispatcher = serviceProvider.GetService<IQueryDispatcher>();
            IEventDispatcher? eventDispatcher = serviceProvider.GetService<IEventDispatcher>();

            // Assert
            Assert.IsNotNull(dispatcher);
            Assert.IsNotNull(commandDispatcher);
            Assert.IsNotNull(queryDispatcher);
            Assert.IsNotNull(eventDispatcher);
        }

        [TestMethod]
        public void AddCqrs_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            void action() => ConfigureServiceProvider(services =>
            {
                services.AddCqrs(null!);
            });

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(action);
            Assert.AreEqual("setupAction", exception.ParamName);
        }

        [TestMethod]
        public void AddDynamicDispatcher_RegistersAllDynamicDispatcherInterfaces()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDynamicDispatcher();
                });
            });

            // Act
            IDynamicDispatcher? dispatcher = serviceProvider.GetService<IDynamicDispatcher>();
            IDynamicCommandDispatcher? commandDispatcher = serviceProvider.GetService<IDynamicCommandDispatcher>();
            IDynamicEventDispatcher? eventDispatcher = serviceProvider.GetService<IDynamicEventDispatcher>();
            IDynamicQueryDispatcher? queryDispatcher = serviceProvider.GetService<IDynamicQueryDispatcher>();

            // Assert
            Assert.IsNotNull(dispatcher);
            Assert.IsNotNull(commandDispatcher);
            Assert.IsNotNull(eventDispatcher);
            Assert.IsNotNull(queryDispatcher);
        }

        [TestMethod]
        public void AddDispatcher_RegistersSingleDispatcherInstance()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                });
            });

            // Act
            IDispatcher dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            IQueryDispatcher queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            IEventDispatcher eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            // Assert
            Assert.AreEqual(dispatcher, commandDispatcher);
            Assert.AreEqual(dispatcher, queryDispatcher);
            Assert.AreEqual(dispatcher, eventDispatcher);
        }

        [TestMethod]
        public void AddDynamicDispatcher_RegistersSingleDynamicDispatcherInstance()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDynamicDispatcher();
                });
            });

            // Act
            IDynamicDispatcher dispatcher = serviceProvider.GetRequiredService<IDynamicDispatcher>();
            IDynamicCommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();
            IDynamicEventDispatcher eventDispatcher = serviceProvider.GetRequiredService<IDynamicEventDispatcher>();
            IDynamicQueryDispatcher queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

            // Assert
            Assert.AreEqual(dispatcher, commandDispatcher);
            Assert.AreEqual(dispatcher, eventDispatcher);
            Assert.AreEqual(dispatcher, queryDispatcher);
        }

        [TestMethod]
        public void AddHandlersFromAssembly_RegistersHandlers()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddScoped(_ => new Queue<Type>());
                services.AddCqrs(builder =>
                {
                    builder.AddHandlersFromAssembly(GetType().Assembly);
                });
            });

            // Act
            var sampleCommandHandler = serviceProvider.GetService<ICommandHandler<SampleCommand>>();
            var sampleQueryHandler = serviceProvider.GetService<IQueryHandler<SampleQuery, SampleQueryResult>>();
            var sampleEventHandlers = serviceProvider.GetServices<IEventHandler<SampleEvent>>();

            // Assert
            Assert.IsNotNull(sampleCommandHandler);
            Assert.IsNotNull(sampleQueryHandler);
            Assert.IsTrue(sampleEventHandlers.Any());
        }

        [TestMethod]
        public void AddInterceptorsFromAssembly_RegistersInterceptors()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddScoped(_ => new Queue<Type>());
                services.AddCqrs(builder =>
                {
                    builder.AddHandlersFromAssembly(GetType().Assembly);
                });
            });

            // Act
            var sampleCommandInterceptors = serviceProvider.GetServices<ICommandInterceptor<SampleCommand>>();
            var sampleQueryInterceptors = serviceProvider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>();

            // Assert
            Assert.IsTrue(sampleCommandInterceptors.Any());
            Assert.IsTrue(sampleQueryInterceptors.Any());
        }

        [TestMethod]
        public void GetCommandHandler_GivenCommandWithoutHandler_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                    builder.AddHandlersFromAssembly(GetType().Assembly);
                });
            });

            var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetCommandHandler(typeof(CommandWithoutHandler));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"No handler found for command with type '{typeof(CommandWithoutHandler)}'.", exception.Message);
        }

        [TestMethod]
        public void GetCommandHandler_GivenCommandWithMultipleHandlers_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                });

                services.AddScoped<ICommandHandler<CommandWithMultipleHandlers>, CommandWithMultipleHandlersFirstHandler>();
                services.AddScoped<ICommandHandler<CommandWithMultipleHandlers>, CommandWithMultipleHandlersSecondHandler>();
            });

            var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetCommandHandler(typeof(CommandWithMultipleHandlers));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"More than one handler found for command with type '{typeof(CommandWithMultipleHandlers)}'.", exception.Message);
        }

        [TestMethod]
        public void GetQueryHandler_GivenQueryWithoutHandler_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                    builder.AddHandlersFromAssembly(GetType().Assembly);
                });
            });

            var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetQueryHandler(typeof(QueryWithoutHandler), typeof(SampleQueryResult));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"No handler found for query with type '{typeof(QueryWithoutHandler)}' and result type '{typeof(SampleQueryResult)}'.", exception.Message);
        }

        [TestMethod]
        public void GetQueryHandler_GivenQueryWithMultipleHandlers_ThrowsInvalidOperationException()
        {
            // Arrange
            using var serviceProvider = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddDispatcher();
                });

                services.AddScoped<IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>, QueryWithMultipleHandlersFirstHandler>();
                services.AddScoped<IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>, QueryWithMultipleHandlersSecondHandler>();
            });

            var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

            // Act
            void action() => handlerRegistry.GetQueryHandler(typeof(QueryWithMultipleHandlers), typeof(SampleQueryResult));

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(action);
            Assert.AreEqual($"More than one handler found for query with type '{typeof(QueryWithMultipleHandlers)}' and result type '{typeof(SampleQueryResult)}'.", exception.Message);
        }
    }
}
