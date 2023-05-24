using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture.Queries;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class QueryInterceptorTests
{
    private readonly Queue<Type> _log = new();

    [TestMethod]
    public async Task DispatchAsync_GivenSampleQuery_RunsInterceptorsInExpectedOrder()
    {
        // Arrange
        using var provider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.AreEqual(typeof(SampleQueryInterceptorWithHighestPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryInterceptorWithHighestMinusThreePriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryInterceptorWithVeryHighPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryInterceptorWithAboveNormalPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryInterceptorWithBelowNormalPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryInterceptorWithLowPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleQueryHandler), _log.Dequeue());
    }

    [TestMethod]
    public async Task InterceptAsync_WithFaultingQuery_ThrowsExpectedException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddQueryHandler<FaultingQuery, SampleQueryResult, FaultingQueryHandler>();
                builder.AddQueryInterceptor<FaultingQuery, SampleQueryResult, FaultingQueryInterceptor>();
            });
        });

        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync(new FaultingQuery());

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        Assert.AreEqual("There was an error.", exception.Message);
    }

    private ServiceProvider CreateServiceProviderWithDefaultConfiguration()
    {
        return ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddQueryHandler<SampleQuery, SampleQueryResult, SampleQueryHandler>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithBelowNormalPriority>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestMinusThreePriority>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithAboveNormalPriority>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithLowPriority>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithVeryHighPriority>();
                builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestPriority>();
            });
            services.AddScoped(_ => _log);
        });
    }
}
