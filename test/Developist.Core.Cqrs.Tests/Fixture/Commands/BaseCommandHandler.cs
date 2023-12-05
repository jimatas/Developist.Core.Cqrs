namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class BaseCommandHandler : IBaseCommandHandler
{
    private readonly Queue<object> _log;

    public BaseCommandHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(BaseCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
