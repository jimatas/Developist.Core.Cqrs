namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandInterceptor : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandInterceptor(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        SampleCommand command,
        CommandHandlerDelegate<SampleCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
