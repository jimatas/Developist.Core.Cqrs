namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class DerivedCommandHandler : ICommandHandler<DerivedCommand>
{
    private readonly Queue<object> _log;

    public DerivedCommandHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(DerivedCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
