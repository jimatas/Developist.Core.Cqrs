namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class SampleCommandInterceptorWithImplicitNormalPriority : ICommandInterceptor<SampleCommand>
{
    private readonly Queue<object> _log;

    public SampleCommandInterceptorWithImplicitNormalPriority(Queue<object> log) => _log = log;

    public Task InterceptAsync(
        SampleCommand command,
        CommandHandlerDelegate<SampleCommand> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(command, cancellationToken);
    }
}
