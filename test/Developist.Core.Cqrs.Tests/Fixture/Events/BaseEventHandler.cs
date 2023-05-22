using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class BaseEventHandler : IEventHandler<BaseEvent>
{
    private readonly Queue<Type> _log;

    public BaseEventHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(BaseEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
