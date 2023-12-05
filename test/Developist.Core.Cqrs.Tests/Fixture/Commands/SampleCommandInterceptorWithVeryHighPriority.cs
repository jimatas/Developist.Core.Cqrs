namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

[PipelinePriority(PriorityLevel.VeryHigh)]
public class SampleCommandInterceptorWithVeryHighPriority : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandInterceptorWithVeryHighPriority(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        SampleCommand command,
        CommandHandlerDelegate<SampleCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
