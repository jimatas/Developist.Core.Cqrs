namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandHandler : ICommandHandler<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandHandler(Queue<object> log) => _log = log;

    public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.CompletedTask;
    }
}
