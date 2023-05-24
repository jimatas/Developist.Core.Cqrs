using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class AnotherSampleEventHandler : IEventHandler<SampleEvent>
{
    private readonly Queue<Type> _log;

    public AnotherSampleEventHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
