using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class CommandInterceptorTests
    {
        #region Fixture
        public class SampleCommand : ICommand { }

        public class SampleCommandHandler : ICommandHandler<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandHandler(Queue<Type> log) => this.log = log;
            public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return Task.CompletedTask;
            }
        }

        public class SampleCommandInterceptorWithHighestPriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithHighestPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Highest;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleCommandInterceptorWithLowestPriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithLowestPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Lowest;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleCommandInterceptorWithLowestPlusOnePriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithLowestPlusOnePriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.Lowest + 1;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleCommandInterceptorWithNormalPriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithNormalPriority(Queue<Type> log) => this.log = log;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleCommandInterceptorWithVeryLowPriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithVeryLowPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.VeryLow;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
            {
                log.Enqueue(GetType());
                return next();
            }
        }

        public class SampleCommandInterceptorWithVeryHighPriority : ICommandInterceptor<SampleCommand>
        {
            private readonly Queue<Type> log;
            public SampleCommandInterceptorWithVeryHighPriority(Queue<Type> log) => this.log = log;
            public PriorityLevel Priority => PriorityLevel.VeryHigh;
            public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
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
                    builder.AddDispatcher();
                    builder.AddDynamicDispatcher();
                    builder.AddCommandHandler<SampleCommand, SampleCommandHandler>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithHighestPriority>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPriority>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPlusOnePriority>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithNormalPriority>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryLowPriority>();
                    builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryHighPriority>();
                });
                services.AddScoped(_ => log);
            });
        }
        #endregion

        [TestMethod]
        public async Task DispatchAsync_GivenSampleCommand_RunsInterceptorsInExpectedOrder()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new SampleCommand());

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
        public async Task DynamicDispatchAsync_GivenSampleCommand_RunsInterceptorsInExpectedOrder()
        {
            // Arrange
            using var serviceProvider = CreateServiceProviderWithDefaultConfiguration();
            var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new SampleCommand());

            // Assert
            Assert.AreEqual(typeof(SampleCommandInterceptorWithHighestPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryHighPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithNormalPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryLowPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPlusOnePriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPriority), log.Dequeue());
            Assert.AreEqual(typeof(SampleCommandHandler), log.Dequeue());
        }
    }
}
