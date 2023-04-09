using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class QueryTests
{
    #region Fixture
    public record QueryResult;
    public record BaseQuery : IQuery<QueryResult>;
    public record DerivedQuery : BaseQuery;
    public record FaultingQuery : IQuery<QueryResult>;

    public class BaseQueryHandler : IQueryHandler<BaseQuery, QueryResult>
    {
        private readonly Queue<Type> _log;
        public BaseQueryHandler(Queue<Type> log) => _log = log;
        public Task<QueryResult> HandleAsync(BaseQuery query, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.FromResult(new QueryResult());
        }
    }

    public class DerivedQueryHandler : IQueryHandler<DerivedQuery, QueryResult>
    {
        private readonly Queue<Type> _log;
        public DerivedQueryHandler(Queue<Type> log) => _log = log;
        public Task<QueryResult> HandleAsync(DerivedQuery query, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.FromResult(new QueryResult());
        }
    }

    public class FaultingQueryHandler : IQueryHandler<FaultingQuery, QueryResult>
    {
        public Task<QueryResult> HandleAsync(FaultingQuery query, CancellationToken cancellationToken)
        {
            throw new ApplicationException("There was an error.");
        }
    }
    #endregion

    #region Setup
    private readonly Queue<Type> _log = new();

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
                builder.AddQueryHandler<BaseQuery, QueryResult, BaseQueryHandler>();
                builder.AddQueryHandler<DerivedQuery, QueryResult, DerivedQueryHandler>();
            });
            services.AddScoped(_ => _log);
        });
    }
    #endregion

    [TestMethod]
    public async Task DispatchAsync_GivenNull_ThrowsNullArgumentException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync<BaseQuery, QueryResult>(null!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual("Value cannot be null. (Parameter 'query')", exception.Message);
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenNull_ThrowsNullArgumentException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync((BaseQuery)null!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual("Value cannot be null. (Parameter 'query')", exception.Message);
    }

    [TestMethod]
    public async Task DispatchAsync_GivenBaseQuery_DispatchesToBaseQueryHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        await queryDispatcher.DispatchAsync<BaseQuery, QueryResult>(new BaseQuery());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseQueryHandler), _log.Single());
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenBaseQuery_DispatchesToBaseQueryHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

        // Act
        await queryDispatcher.DispatchAsync(new BaseQuery());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(BaseQueryHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenDerivedQuery_DispatchesToDerivedQueryHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        await queryDispatcher.DispatchAsync<DerivedQuery, QueryResult>(new DerivedQuery());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedQueryHandler), _log.Single());
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenDerivedQuery_DispatchesToDerivedQueryHandler()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

        // Act
        await queryDispatcher.DispatchAsync(new DerivedQuery());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedQueryHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenFaultingQuery_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Dispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatcher();
                builder.AddQueryHandler<FaultingQuery, QueryResult, FaultingQueryHandler>();
            });
            services.AddScoped(_ => _log);
            services.AddScoped(_ => loggerMock.Object);
        });

        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync<FaultingQuery, QueryResult>(new FaultingQuery());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        loggerMock.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during query dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    [TestMethod]
    public async Task DynamicDispatchAsync_GivenFaultingQuery_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DynamicDispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDynamicDispatcher();
                builder.AddQueryHandler<FaultingQuery, QueryResult, FaultingQueryHandler>();
            });
            services.AddScoped(_ => _log);
            services.AddScoped(_ => loggerMock.Object);
        });

        var queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync(new FaultingQuery());

        // Assert
        await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        loggerMock.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Equals("Unhandled exception during query dispatch: There was an error.")),
            It.Is<ApplicationException>(exception => exception.Message.Equals("There was an error.")),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }
}
