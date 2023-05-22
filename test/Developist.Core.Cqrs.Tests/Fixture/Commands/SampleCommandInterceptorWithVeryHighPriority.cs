using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandInterceptorWithVeryHighPriority : ICommandInterceptor<SampleCommand>, IPrioritizable
{
    private readonly Queue<Type> _log;

    public SampleCommandInterceptorWithVeryHighPriority(Queue<Type> log) => _log = log;

    public PriorityLevel Priority => PriorityLevel.VeryHigh;

    public Task InterceptAsync(SampleCommand command, CommandHandlerDelegate<SampleCommand> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(command, cancellationToken);
    }
}
