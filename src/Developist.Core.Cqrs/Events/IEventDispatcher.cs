namespace Developist.Core.Cqrs;

/// <summary>
/// Represents an event dispatcher responsible for dispatching events.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches the specified event asynchronously to its corresponding handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to dispatch.</typeparam>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
