namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a handler for processing commands of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}
