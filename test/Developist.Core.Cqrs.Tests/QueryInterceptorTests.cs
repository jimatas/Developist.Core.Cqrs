using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using static Developist.Core.Cqrs.Tests.QueryTests;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class QueryInterceptorTests
{
    #region Fixture
    public record SampleQuery : IQuery<SampleQueryResult>;
    public record SampleQueryResult;

    public class SampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> _log;
        public SampleQueryHandler(Queue<Type> log) => _log = log;
        public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.FromResult(new SampleQueryResult());
        }
    }

    public class SampleQueryInterceptorWithBelowNormalPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithBelowNormalPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.BelowNormal;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithHighestMinusThreePriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithHighestMinusThreePriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Highest - 3;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithAboveNormalPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithAboveNormalPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.AboveNormal;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithLowPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithLowPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Low;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithVeryHighPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithVeryHighPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.VeryHigh;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithHighestPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleQueryInterceptorWithHighestPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Highest;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class FaultingQueryInterceptor : IQueryInterceptor<FaultingQuery, QueryResult>
    {
        public Task<QueryResult> InterceptAsync(FaultingQuery query, HandlerDelegate<QueryResult> next, CancellationToken cancellationToken)
        {
            return next();
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
    #endregion

    [TestMethod]
    public async Task DispatchAsync_GivenSampleQuery_RunsInterceptorsInExpectedOrder()
    {
        // Arrange
        using var provider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();

        // Act
        var result = await queryDispatcher.DispatchAsync<SampleQuery, SampleQueryResult>(new SampleQuery());

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
    public async Task DynamicDispatchAsync_GivenSampleQuery_RunsInterceptorsInExpectedOrder()
    {
        // Arrange
        using var provider = CreateServiceProviderWithDefaultConfiguration();
        var queryDispatcher = provider.GetRequiredService<IDynamicQueryDispatcher>();

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
    public async Task DynamicInterceptAsync_WithFaultingQuery_ThrowsCorrectException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDynamicDispatcher();
                builder.AddQueryHandler<FaultingQuery, QueryResult, FaultingQueryHandler>();
                builder.AddQueryInterceptor<FaultingQuery, QueryResult, FaultingQueryInterceptor>();
            });
        });

        var queryDispatcher = serviceProvider.GetRequiredService<IDynamicQueryDispatcher>();

        // Act
        var action = () => queryDispatcher.DispatchAsync(new FaultingQuery());

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        Assert.AreEqual("There was an error.", exception.Message);
    }
}
