using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandInterceptorWithImplicitNormalPriority : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<Type> _log;

    public SampleCommandInterceptorWithImplicitNormalPriority(Queue<Type> log) => _log = log;

    public Task InterceptAsync(SampleCommand command, CommandHandlerDelegate<SampleCommand> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(command, cancellationToken);
    }
}
