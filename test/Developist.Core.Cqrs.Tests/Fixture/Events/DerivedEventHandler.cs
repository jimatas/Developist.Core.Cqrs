namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class DerivedEventHandler : IEventHandler<DerivedEvent>
{
    private readonly Queue<Type> _log;

    public DerivedEventHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(DerivedEvent @event, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
