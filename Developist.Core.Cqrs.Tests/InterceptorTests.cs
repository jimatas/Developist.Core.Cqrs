using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Developist.Core.Cqrs.Queries;
using Developist.Core.Cqrs.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class InterceptorTests
    {
        private readonly Queue<Type> log = new();

        private ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddCqrs(builder =>
                builder.AddDefaultDispatcher()
                    .AddDefaultRegistry()
                    .AddHandlersFromAssembly(Assembly.GetExecutingAssembly())
                    .AddInterceptorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped(_ => log);
            return services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task DispatchAsync_GivenSampleCommand_RunsInterceptorsInExpectedOrder()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            ICommandDispatcher dispatcher = provider.GetRequiredService<ICommandDispatcher>();

            // Act
            await dispatcher.DispatchAsync(new SampleCommand());

            // Assert
            Assert.AreEqual(typeof(SampleCommandInterceptorWithHighestPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryHighPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithNormalPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryLowPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPlusOnePriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandHandler), log.Dequeue());
        }

        [TestMethod]
        public async Task DispatchAsync_GivenSampleQuery_RunsInterceptorsInExpectedOrder()
        {
            // Arrange
            using var provider = CreateServiceProvider();
            IQueryDispatcher dispatcher = provider.GetRequiredService<IQueryDispatcher>();

            // Act
            SampleQueryResult result = await dispatcher.DispatchAsync(new SampleQuery());

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
