namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a command dispatcher responsible for dispatching commands.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches the specified command asynchronously to its corresponding handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;
}
