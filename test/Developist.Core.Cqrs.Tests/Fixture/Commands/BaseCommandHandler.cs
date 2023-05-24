using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class BaseCommandHandler : ICommandHandler<BaseCommand>
{
    private readonly Queue<Type> _log;

    public BaseCommandHandler(Queue<Type> log) => _log = log;

    public Task HandleAsync(BaseCommand command, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.CompletedTask;
    }
}
