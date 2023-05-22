using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class DerivedCommandHandler : ICommandHandler<DerivedCommand>
{
    private readonly Queue<Type> _log;

    public DerivedCommandHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(DerivedCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
