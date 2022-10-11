using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class QueryInterceptorTests
    {
        #region Fixture
        public class SampleQuery : IQuery<SampleQueryResult> { }
        public record SampleQueryResult { }

        public class SampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryHandler(Queue<Type> log) => this.log = log;
            public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.FromResult(new SampleQueryResult());
            }
        }

        public class SampleQueryInterceptorWithLowPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithLowPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Low;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleQueryInterceptorWithHighestMinusThreePriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithHighestMinusThreePriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Highest - 3;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleQueryInterceptorWithHighPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithHighPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.High;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleQueryInterceptorWithLowerPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithLowerPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Lower;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleQueryInterceptorWithVeryHighPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithVeryHighPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.VeryHigh;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleQueryInterceptorWithHighestPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
        {
            private readonly Queue<Type> log;
            public SampleQueryInterceptorWithHighestPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Highest;
            public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }
        #endregion

        #region Setup
        private readonly Queue<Type> log = new();

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
                    builder.AddDefaultDispatcher();
                    builder.AddQueryHandler<SampleQuery, SampleQueryResult, SampleQueryHandler>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithLowPriority>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestMinusThreePriority>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighPriority>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithLowerPriority>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithVeryHighPriority>();
                    builder.AddQueryInterceptor<SampleQuery, SampleQueryResult, SampleQueryInterceptorWithHighestPriority>();
                });
                services.AddScoped(_ => log);
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
            SampleQueryResult result = await queryDispatcher.DispatchAsync(new SampleQuery());

            // Assert
            Assert.AreEqual(typeof(SampleQueryInterceptorWithHighestPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryInterceptorWithHighestMinusThreePriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryInterceptorWithVeryHighPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryInterceptorWithHighPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryInterceptorWithLowPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryInterceptorWithLowerPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleQueryHandler), log.Dequeue());
        }
    }
}
