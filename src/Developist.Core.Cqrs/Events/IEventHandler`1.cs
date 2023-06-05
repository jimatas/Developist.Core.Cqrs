using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the contract for an event handler that handles events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to be handled, which must implement the <see cref="IEvent"/> interface.</typeparam>
    public interface IEventHandler<TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// Handles an event asynchronously.
        /// </summary>
        /// <param name="event">The event to be handled.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
