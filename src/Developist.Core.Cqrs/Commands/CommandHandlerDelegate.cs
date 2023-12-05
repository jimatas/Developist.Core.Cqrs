namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a delegate for handling commands of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <param name="command">The command to handle.</param>
/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task CommandHandlerDelegate<TCommand>(TCommand command, CancellationToken cancellationToken)
    where TCommand : ICommand;
