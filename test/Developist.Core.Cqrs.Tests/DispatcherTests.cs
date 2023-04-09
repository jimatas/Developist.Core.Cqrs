using Developist.Core.Cqrs.Infrastructure;

namespace Developist.Core.Cqrs.Tests;

[TestClass]
public class DispatcherTests
{
    [TestMethod]
    public void InitializeDispatcher_GivenNullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry registry = null!;

        // Act
        var action = () => new Dispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }

    [TestMethod]
    public void InitializeDynamicDispatcher_GivenNullRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        IHandlerRegistry registry = null!;

        // Act
        var action = () => new DynamicDispatcher(registry);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(registry), exception.ParamName);
    }
}
