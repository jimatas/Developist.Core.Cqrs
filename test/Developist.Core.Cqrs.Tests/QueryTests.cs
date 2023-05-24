using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class QueryTests
{
    private readonly Queue<Type> _log = new();

    [TestMethod]
    public async Task DispatchAsync_GivenNull_ThrowsNullArgumentException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync((BaseQuery)null!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual("Value cannot be null. (Parameter 'query')", exception.Message);
    }

    [TestMethod]
    public async Task DispatchAsyncOverload_GivenNull_ThrowsNullArgumentException()
    {
        // Arrange
        using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = (QueryDispatcher)serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync<BaseQuery, SampleQueryResult>(null!);

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
        await queryDispatcher.DispatchAsync(new DerivedQuery());

        // Assert
        Assert.AreEqual(1, _log.Count);
        Assert.AreEqual(typeof(DerivedQueryHandler), _log.Single());
    }

    [TestMethod]
    public async Task DispatchAsync_GivenFaultingQuery_LogsExceptionAndThrowsIt()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<QueryDispatcher>>();
        loggerMock.Setup(logger => logger.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddQueryHandler<FaultingQuery, SampleQueryResult, FaultingQueryHandler>();
            });
            services.AddScoped(_ => _log);
            services.AddScoped(_ => loggerMock.Object);
        });

        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

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

    private ServiceProvider CreateServiceProviderWithDefaultConfiguration()
    {
        return ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddQueryHandler<BaseQuery, SampleQueryResult, BaseQueryHandler>();
                builder.AddQueryHandler<DerivedQuery, SampleQueryResult, DerivedQueryHandler>();
            });
            services.AddScoped(_ => _log);
        });
    }
}
