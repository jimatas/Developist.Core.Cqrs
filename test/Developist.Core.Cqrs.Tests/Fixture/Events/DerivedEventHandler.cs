namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class DerivedEventHandler : IEventHandler<DerivedEvent>
{
    private readonly Queue<object> _log;

    public DerivedEventHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(DerivedEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
