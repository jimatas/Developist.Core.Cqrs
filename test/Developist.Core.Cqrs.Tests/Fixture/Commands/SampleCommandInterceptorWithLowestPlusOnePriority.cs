namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandInterceptorWithLowestPlusOnePriority : ICommandInterceptor<SampleCommand>, IPrioritizable
{
    private readonly Queue<Type> _log;

    public SampleCommandInterceptorWithLowestPlusOnePriority(Queue<Type> log) => _log = log;

    public PriorityLevel Priority => PriorityLevel.Lowest + 1;

    public Task InterceptAsync(SampleCommand command, CommandHandlerDelegate<SampleCommand> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(command, cancellationToken);
    }
}
