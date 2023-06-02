using Developist.Core.Cqrs.Tests.Fixture.Commands;
using Developist.Core.Cqrs.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class CommandInterceptorTests
{
    private readonly Queue<Type> _log = new();

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
    public async Task InterceptAsync_WithFaultingCommand_ThrowsExpectedException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services =>
        {
            services.AddCqrs(builder =>
            {
                builder.AddDispatchers();
                builder.AddCommandHandler<FaultingCommand, FaultingCommandHandler>();
                builder.AddCommandInterceptor<FaultingCommand, FaultingCommandInterceptor>();
            });
        });

        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

        // Act
        var action = () => commandDispatcher.DispatchAsync(new FaultingCommand());

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
}
