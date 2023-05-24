using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    /// <summary>
    /// Defines the contract for an event dispatcher.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Dispatches an event asynchronously to its registered handlers.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be dispatched, which must implement the <see cref="IEvent"/> interface.</typeparam>
        /// <param name="event">The event to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
