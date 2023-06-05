namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandHandler : ICommandHandler<SampleCommand>
{
    private readonly Queue<Type> _log;

    public SampleCommandHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
