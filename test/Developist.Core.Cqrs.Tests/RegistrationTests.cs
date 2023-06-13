using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Developist.Core.Cqrs.Tests.Fixture.Events;
using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class RegistrationTests
{
    [TestMethod]
    public void AddCqrs_GivenNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Action<CqrsBuilder>? configureBuilder = null;
        
        // Act
        var action = () =>
        {
            using var _ = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddCqrs(configureBuilder));
        };

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(configureBuilder), exception.ParamName);
    }

    [TestMethod]
    public void AddDispatcher_ByDefault_RegistersAllDispatchers()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
            });
        });

        // Act
        var dispatcher = serviceProvider.GetService<IDispatcher>();
        var commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
        var eventDispatcher = serviceProvider.GetService<IEventDispatcher>();
        var queryDispatcher = serviceProvider.GetService<IQueryDispatcher>();

        // Assert
        Assert.IsNotNull(dispatcher);
        Assert.IsNotNull(commandDispatcher);
        Assert.IsNotNull(eventDispatcher);
        Assert.IsNotNull(queryDispatcher);
    }

    [TestMethod]
    public void AddDispatcher_ByDefault_RegistersExpectedDispatcherInstances()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
            });
        });

        // Act
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
        var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Assert
        Assert.IsInstanceOfType(dispatcher, typeof(Dispatcher));
        Assert.IsInstanceOfType(commandDispatcher, typeof(CommandDispatcher));
        Assert.IsInstanceOfType(eventDispatcher, typeof(EventDispatcher));
        Assert.IsInstanceOfType(queryDispatcher, typeof(QueryDispatcher));
    }

    [TestMethod]
    public void AddHandlersFromAssembly_GivenNullAssembly_ThrowsArgumentNullException()
    {
        // Arrange
        Assembly? assembly = null;

        // Act
        var action = () =>
        {
            using var _ = ServiceProviderHelper.ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddHandlersFromAssembly(assembly);
                });
            });
        };

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(assembly), exception.ParamName);
    }

    [TestMethod]
    public void AddHandlersFromAssembly_ByDefault_RegistersBothHandlersAndInterceptors()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
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
        var sampleCommandInterceptors = serviceProvider.GetServices<ICommandInterceptor<SampleCommand>>();
        var sampleQueryInterceptors = serviceProvider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>();

        // Assert
        Assert.IsNotNull(sampleCommandHandler);
        Assert.IsNotNull(sampleQueryHandler);
        Assert.IsTrue(sampleEventHandlers.Any());
        Assert.IsTrue(sampleCommandInterceptors.Any());
        Assert.IsTrue(sampleQueryInterceptors.Any());
    }

    [TestMethod]
    public void AddHandlersFromAssembly_ByDefault_RegistersGenericCommandInterceptor()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddScoped(_ => new Queue<Type>());
            services.AddCqrs(builder =>
            {
                builder.AddHandlersFromAssembly(GetType().Assembly);
            });
        });

        // Act
        var genericCommandInterceptor = serviceProvider.GetServices<ICommandInterceptor<SampleCommand>>()
            .FirstOrDefault(interceptor => interceptor is GenericCommandInterceptor<SampleCommand>);

        // Assert
        Assert.IsNotNull(genericCommandInterceptor);
    }

    [TestMethod]
    public void AddHandlersFromAssembly_ByDefault_RegistersGenericQueryInterceptor()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddScoped(_ => new Queue<Type>());
            services.AddCqrs(builder =>
            {
                builder.AddHandlersFromAssembly(GetType().Assembly);
            });
        });

        // Act
        var genericQueryInterceptor = serviceProvider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>()
            .FirstOrDefault(interceptor => interceptor is GenericQueryInterceptor<SampleQuery, SampleQueryResult>);

        // Assert
        Assert.IsNotNull(genericQueryInterceptor);
    }

    [TestMethod]
    public void AddHandlersFromAssembly_GivenAssemblyWithPartiallyClosedHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.ExportedTypes).Returns(new[] { typeof(PartiallyClosedQueryHandler<>) });

        // Act
        var action = () =>
        {
            using var _ = ServiceProviderHelper.ConfigureServiceProvider(services =>
            {
                services.AddCqrs(builder =>
                {
                    builder.AddHandlersFromAssembly(mockAssembly.Object);
                });
            });
        };

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual("Types that only partially close either the IQueryHandler or IQueryInterceptor generic interface are not supported.", exception.Message);
    }

    [TestMethod]
    public void GetCommandHandler_GivenCommandWithoutHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddHandlersFromAssembly(GetType().Assembly);
            });
        });

        var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => handlerRegistry.GetCommandHandler<CommandWithoutHandler>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for command with type '{typeof(CommandWithoutHandler)}'.", exception.Message);
    }

    [TestMethod]
    public void GetCommandHandler_GivenCommandWithMultipleHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
            });

            services.AddScoped<ICommandHandler<CommandWithMultipleHandlers>, CommandWithMultipleHandlersFirstHandler>();
            services.AddScoped<ICommandHandler<CommandWithMultipleHandlers>, CommandWithMultipleHandlersSecondHandler>();
        });

        var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => handlerRegistry.GetCommandHandler<CommandWithMultipleHandlers>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"More than one handler found for command with type '{typeof(CommandWithMultipleHandlers)}'.", exception.Message);
    }

    [TestMethod]
    public void GetQueryHandler_GivenQueryWithoutHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddHandlersFromAssembly(GetType().Assembly);
            });
        });

        var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => handlerRegistry.GetQueryHandler<QueryWithoutHandler, SampleQueryResult>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for query with type '{typeof(QueryWithoutHandler)}' and result type '{typeof(SampleQueryResult)}'.", exception.Message);
    }

    [TestMethod]
    public void GetQueryHandler_GivenQueryWithMultipleHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
            });

            services.AddScoped<IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>, QueryWithMultipleHandlersFirstHandler>();
            services.AddScoped<IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>, QueryWithMultipleHandlersSecondHandler>();
        });

        var handlerRegistry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => handlerRegistry.GetQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"More than one handler found for query with type '{typeof(QueryWithMultipleHandlers)}' and result type '{typeof(SampleQueryResult)}'.", exception.Message);
    }
}
