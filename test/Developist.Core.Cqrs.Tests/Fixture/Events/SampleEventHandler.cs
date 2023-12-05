namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class SampleEventHandler : IEventHandler<SampleEvent>
{
    private readonly Queue<object> _log;

    public SampleEventHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
