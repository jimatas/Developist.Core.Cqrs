using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class QueryTests : TestClassBase
{
    [TestMethod]
    public async Task DispatchAsync_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
            });
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
        IQuery<SampleQueryResult> query = null!;

        // Act
        Task action() => dispatcher.DispatchAsync(query);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(query), exception.ParamName);
    }

    [TestMethod]
    public async Task DefaultDispatcher_DispatchAsync_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
            });
        });

        var dispatcher = (DefaultDispatcher)serviceProvider.GetRequiredService<IQueryDispatcher>();
        IQuery<SampleQueryResult> query = null!;

        // Act
        Task action() => dispatcher.DispatchAsync<SampleQuery, SampleQueryResult>(query);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(query), exception.ParamName);
    }

    [TestMethod]
    public async Task DispatchAsync_SampleQuery_DispatchesToSampleQueryHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddQueryHandler<SampleQuery, SampleQueryResult, SampleQueryHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.IsInstanceOfType<SampleQueryHandler>(log.Dequeue());
    }

    [TestMethod]
    public async Task DispatchAsync_BaseQuery_DispatchesToBaseQueryHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddQueryHandler<BaseQuery, SampleQueryResult, BaseQueryHandler>();
                cfg.AddQueryHandler<DerivedQuery, SampleQueryResult, DerivedQueryHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new BaseQuery());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<BaseQueryHandler>(log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_DerivedQuery_DispatchesToDerivedQueryHandler()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddQueryHandler<BaseQuery, SampleQueryResult, BaseQueryHandler>();
                cfg.AddQueryHandler<DerivedQuery, SampleQueryResult, DerivedQueryHandler>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        await dispatcher.DispatchAsync(new DerivedQuery());

        // Assert
        Assert.AreEqual(1, log.Count);
        Assert.IsInstanceOfType<DerivedQueryHandler>(log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_FaultingQueryHandler_LogsExceptionAndThrows()
    {
        // Arrange
        var log = new Queue<object>();

        var logger = new Mock<ILogger<DefaultDispatcher>>();
        logger.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddQueryHandler<SampleQuery, SampleQueryResult, FaultingSampleQueryHandler>();
            });
            services.AddScoped(_ => log);
            services.AddScoped(_ => logger.Object);
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => dispatcher.DispatchAsync(new SampleQuery());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        logger.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during query dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [TestMethod]
    public async Task DispatchAsync_SampleQuery_RunsInterceptorsInExpectedOrder()
    {
        // Arrange
        var log = new Queue<object>();
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(cfg =>
            {
                cfg.AddDefaultDispatcher();
                cfg.AddQueryHandler<SampleQuery, SampleQueryResult, SampleQueryHandler>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithBelowNormalPriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestMinusThreePriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithAboveNormalPriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithLowPriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithVeryHighPriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithImplicitNormalPriority>();
                cfg.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestPriority>();
            });
            services.AddScoped(_ => log);
        });

        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        _ = await dispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.IsInstanceOfType<SampleQueryInterceptorWithHighestPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithHighestMinusThreePriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithVeryHighPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithAboveNormalPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithImplicitNormalPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithBelowNormalPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryInterceptorWithLowPriority>(log.Dequeue());
        Assert.IsInstanceOfType<SampleQueryHandler>(log.Dequeue());
    }
}
