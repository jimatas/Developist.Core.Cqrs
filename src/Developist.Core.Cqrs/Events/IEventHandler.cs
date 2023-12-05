namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a handler for processing events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the specified event asynchronously.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
