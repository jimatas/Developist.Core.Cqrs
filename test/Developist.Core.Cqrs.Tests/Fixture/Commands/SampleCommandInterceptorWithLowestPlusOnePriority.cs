namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

[PipelinePriority(PriorityLevel.Lowest + 1)]
public class SampleCommandInterceptorWithLowestPlusOnePriority : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandInterceptorWithLowestPlusOnePriority(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        SampleCommand command,
        CommandHandlerDelegate<SampleCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
