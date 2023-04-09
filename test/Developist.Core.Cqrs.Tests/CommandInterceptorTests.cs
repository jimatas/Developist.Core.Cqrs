using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using static Developist.Core.Cqrs.Tests.CommandTests;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class CommandInterceptorTests
{
    #region Fixture
    public record SampleCommand : ICommand;

    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        private readonly Queue<Type> _log;
        public SampleCommandHandler(Queue<Type> log) => _log = log;
        public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class SampleCommandInterceptorWithHighestPriority : ICommandInterceptor<SampleCommand>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithHighestPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Highest;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithLowestPriority : ICommandInterceptor<SampleCommand>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithLowestPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Lowest;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithLowestPlusOnePriority : ICommandInterceptor<SampleCommand>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithLowestPlusOnePriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.Lowest + 1;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithImplicitNormalPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithImplicitNormalPriority(Queue<Type> log) => _log = log;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithVeryLowPriority : ICommandInterceptor<SampleCommand>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithVeryLowPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.VeryLow;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithVeryHighPriority : ICommandInterceptor<SampleCommand>, IPrioritizable
    {
        private readonly Queue<Type> _log;
        public SampleCommandInterceptorWithVeryHighPriority(Queue<Type> log) => _log = log;
        public PriorityLevel Priority => PriorityLevel.VeryHigh;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            _log.Enqueue(GetType());
            return next();
        }
    }

    public class FaultingCommandInterceptor : ICommandInterceptor<FaultingCommand>
    {
        public Task InterceptAsync(FaultingCommand command, HandlerDelegate next, CancellationToken cancellationToken)
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
                builder.AddCommandHandler<SampleCommand, SampleCommandHandler>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithHighestPriority>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPriority>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithLowestPlusOnePriority>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithImplicitNormalPriority>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryLowPriority>();
                builder.AddCommandInterceptor<SampleCommand, SampleCommandInterceptorWithVeryHighPriority>();
            });
            services.AddScoped(_ => _log);
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
        Assert.AreEqual(typeof(SampleCommandInterceptorWithHighestPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryHighPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithImplicitNormalPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryLowPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPlusOnePriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandHandler), _log.Dequeue());
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
        Assert.AreEqual(typeof(SampleCommandInterceptorWithHighestPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryHighPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithImplicitNormalPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithVeryLowPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPlusOnePriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandInterceptorWithLowestPriority), _log.Dequeue());
        Assert.AreEqual(typeof(SampleCommandHandler), _log.Dequeue());
    }

    [TestMethod]
    public async Task DynamicInterceptAsync_WithFaultingCommand_ThrowsCorrectException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDynamicDispatcher();
                builder.AddCommandHandler<FaultingCommand, FaultingCommandHandler>();
                builder.AddCommandInterceptor<FaultingCommand, FaultingCommandInterceptor>();
            });
        });

        var commandDispatcher = serviceProvider.GetRequiredService<IDynamicCommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync(new FaultingCommand());

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ApplicationException>(action);
        Assert.AreEqual("There was an error.", exception.Message);
    }
}
