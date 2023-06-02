using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the contract for a command dispatcher.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches a command asynchronously to its registered handler.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to be dispatched, which must implement the <see cref="ICommand"/> interface.</typeparam>
        /// <param name="command">The command to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;
    }
}
