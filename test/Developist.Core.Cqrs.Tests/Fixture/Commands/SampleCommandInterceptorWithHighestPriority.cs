namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

[PipelinePriority(PriorityLevel.Highest)]
public class SampleCommandInterceptorWithHighestPriority : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandInterceptorWithHighestPriority(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        SampleCommand command,
        CommandHandlerDelegate<SampleCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
