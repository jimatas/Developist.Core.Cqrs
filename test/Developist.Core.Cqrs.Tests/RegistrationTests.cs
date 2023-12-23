using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class RegistrationTests : TestClassBase
{
    [TestMethod]
    public void DefaultDispatcher_Initialize_NullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry registry = null!;

        // Act
        var action = () => new DefaultDispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }

    [TestMethod]
    public void DefaultHandlerRegistry_Initialize_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;

        // Act
        var action = () => new DefaultHandlerRegistry(serviceProvider);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(serviceProvider), exception.ParamName);
    }

    [TestMethod]
    public void AddDefaultDispatcher_RegistersAllDispatcherInterfaces()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddCqrs(cfg => action(cfg)));

        // Act
        static void action(CqrsConfigurator cfg) => cfg.AddDefaultDispatcher();

        // Assert
        Assert.IsNotNull(serviceProvider.GetService<IDispatcher>());
        Assert.IsNotNull(serviceProvider.GetService<ICommandDispatcher>());
        Assert.IsNotNull(serviceProvider.GetService<IQueryDispatcher>());
    }

    [TestMethod]
    public void AddDefaultDispatcher_RegistersDefaultDispatcherAsImplementation()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddCqrs(cfg => action(cfg)));

        // Act
        static void action(CqrsConfigurator cfg) => cfg.AddDefaultDispatcher();

        // Assert
        Assert.IsInstanceOfType<DefaultDispatcher>(serviceProvider.GetRequiredService<IDispatcher>());
        Assert.IsInstanceOfType<DefaultDispatcher>(serviceProvider.GetRequiredService<ICommandDispatcher>());
        Assert.IsInstanceOfType<DefaultDispatcher>(serviceProvider.GetRequiredService<IQueryDispatcher>());
    }

    [TestMethod]
    public void AddHandlersFromAssembly_NullAssembly_ThrowsArgumentNullException()
    {
        // Arrange
        Assembly assembly = null!;

        // Act
        void action()
        {
            using var _ = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(cfg =>
                {
                    cfg.AddHandlersFromAssembly(assembly);
                });
            });
        };

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(assembly), exception.ParamName);
    }

    [TestMethod]
    public void AddHandlersFromAssembly_RegistersBothHandlersAndInterceptors()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddCqrs(cfg => action(cfg)));

        // Act
        static void action(CqrsConfigurator cfg) => cfg.AddHandlersFromAssembly(typeof(RegistrationTests).Assembly);

        // Assert
        Assert.IsNotNull(serviceProvider.GetService<ICommandHandler<SampleCommand>>());
        Assert.IsNotNull(serviceProvider.GetService<IQueryHandler<SampleQuery, SampleQueryResult>>());
        Assert.IsTrue(serviceProvider.GetServices<ICommandInterceptor<SampleCommand>>().Any());
        Assert.IsTrue(serviceProvider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>().Any());
    }

    [TestMethod]
    public void AddHandlersFromAssembly_RegistersGenericCommandInterceptor()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddCqrs(cfg => action(cfg)));

        // Act
        static void action(CqrsConfigurator cfg) => cfg.AddHandlersFromAssembly(typeof(RegistrationTests).Assembly);

        // Assert
        Assert.IsTrue(serviceProvider.GetServices<ICommandInterceptor<SampleCommand>>()
            .Any(interceptor => interceptor is GenericCommandInterceptor<SampleCommand>));
    }

    [TestMethod]
    public void AddHandlersFromAssembly_RegistersGenericQueryInterceptor()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddCqrs(cfg => action(cfg)));

        // Act
        static void action(CqrsConfigurator cfg) => cfg.AddHandlersFromAssembly(typeof(RegistrationTests).Assembly);

        // Assert
        Assert.IsTrue(serviceProvider.GetServices<IQueryInterceptor<SampleQuery, SampleQueryResult>>()
            .Any(interceptor => interceptor is GenericQueryInterceptor<SampleQuery, SampleQueryResult>));
    }

    [TestMethod]
    public void AddHandlersFromAssembly_AssemblyWithPartiallyClosedQueryHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var assembly = new Mock<Assembly>();
        assembly.Setup(a => a.ExportedTypes).Returns(new[] { typeof(PartiallyClosedQueryHandler<>) });

        // Act
        void action()
        {
            using var _ = ConfigureServiceProvider(services =>
            {
                services.AddCqrs(cfg =>
                {
                    cfg.AddHandlersFromAssembly(assembly.Object);
                });
            });
        };

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual("Types that partially close the IQueryHandler or IQueryInterceptor generic interfaces are not supported.", exception.Message);
    }

    [TestMethod]
    public void GetCommandHandler_CommandWithoutHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg => services.AddCqrs(cfg => cfg.AddDefaultDispatcher()));
        });

        var registry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => registry.GetCommandHandler<SampleCommand>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for command '{typeof(SampleCommand)}'.", exception.Message);
    }

    [TestMethod]
    public void GetCommandHandler_CommandWithMultipleHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg => cfg.AddDefaultDispatcher());
            services.AddScoped(_ => new Mock<ICommandHandler<SampleCommand>>().Object);
            services.AddScoped(_ => new Mock<ICommandHandler<SampleCommand>>().Object);
        });

        var registry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => registry.GetCommandHandler<SampleCommand>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"More than one handler found for command '{typeof(SampleCommand)}'.", exception.Message);
    }

    [TestMethod]
    public void GetQueryHandler_QueryWithoutHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg => services.AddCqrs(cfg => cfg.AddDefaultDispatcher()));
        });

        var registry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => registry.GetQueryHandler<SampleQuery, SampleQueryResult>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"No handler found for query '{typeof(SampleQuery)}' with result type '{typeof(SampleQueryResult)}'.", exception.Message);
    }

    [TestMethod]
    public void GetQueryHandler_QueryWithMultipleHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg => cfg.AddDefaultDispatcher());
            services.AddScoped(_ => new Mock<IQueryHandler<SampleQuery, SampleQueryResult>>().Object);
            services.AddScoped(_ => new Mock<IQueryHandler<SampleQuery, SampleQueryResult>>().Object);
        });

        var registry = serviceProvider.GetRequiredService<IHandlerRegistry>();

        // Act
        var action = () => registry.GetQueryHandler<SampleQuery, SampleQueryResult>();

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"More than one handler found for query '{typeof(SampleQuery)}' with result type '{typeof(SampleQueryResult)}'.", exception.Message);
    }
}
