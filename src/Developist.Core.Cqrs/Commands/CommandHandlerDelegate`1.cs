using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents a delegate that handles a command asynchronously.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to handle.</typeparam>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public delegate Task CommandHandlerDelegate<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;
}
