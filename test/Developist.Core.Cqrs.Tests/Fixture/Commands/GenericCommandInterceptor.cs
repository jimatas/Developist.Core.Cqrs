namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class GenericCommandInterceptor<TCommand> : ICommandInterceptor<TCommand>
    where TCommand : ICommand
{
    private readonly Queue<object> _log;

    public GenericCommandInterceptor(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        TCommand command,
        CommandHandlerDelegate<TCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
