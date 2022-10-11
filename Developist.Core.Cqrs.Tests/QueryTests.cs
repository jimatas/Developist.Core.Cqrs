using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class QueryTests
    {
        #region Fixture
        public record QueryResult { }
        public class BaseQuery : IQuery<QueryResult> { }
        public class DerivedQuery : BaseQuery { }

        public class BaseQueryHandler : IQueryHandler<BaseQuery, QueryResult>
        {
            private readonly Queue<Type> log;
            public BaseQueryHandler(Queue<Type> log) => this.log = log;
            public Task<QueryResult> HandleAsync(BaseQuery query, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.FromResult(new QueryResult());
            }
        }

        public class DerivedQueryHandler : IQueryHandler<DerivedQuery, QueryResult>
        {
            private readonly Queue<Type> log;
            public DerivedQueryHandler(Queue<Type> log) => this.log = log;
            public Task<QueryResult> HandleAsync(DerivedQuery query, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.FromResult(new QueryResult());
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
                    builder.AddQueryHandler<BaseQuery, QueryResult, BaseQueryHandler>();
                    builder.AddQueryHandler<DerivedQuery, QueryResult, DerivedQueryHandler>();
                });
                services.AddScoped(_ => log);
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
            Task action() => queryDispatcher.DispatchAsync<IQuery<QueryResult>>(null!);

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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(BaseQueryHandler), log.Single());
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
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual(typeof(DerivedQueryHandler), log.Single());
        }
    }
}
