using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    /// <summary>
    /// Defines the contract for a dynamic command dispatcher.
    /// </summary>
    public interface IDynamicCommandDispatcher
    {
        /// <summary>
        /// Dispatches a command asynchronously to its registered handler.
        /// </summary>
        /// <param name="command">The command to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
