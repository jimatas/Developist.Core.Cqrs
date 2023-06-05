namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class SampleEventHandler : IEventHandler<SampleEvent>
{
    private readonly Queue<Type> _log;

    public SampleEventHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
