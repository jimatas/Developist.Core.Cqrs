using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    /// <summary>
    /// Defines the contract for a dynamic event dispatcher.
    /// </summary>
    public interface IDynamicEventDispatcher
    {
        /// <summary>
        /// Dispatches an event asynchronously to its registered handlers.
        /// </summary>
        /// <param name="event">The event to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
