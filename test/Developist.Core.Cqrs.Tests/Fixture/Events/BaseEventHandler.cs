namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class BaseEventHandler : IEventHandler<BaseEvent>
{
    private readonly Queue<object> _log;

    public BaseEventHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(BaseEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
