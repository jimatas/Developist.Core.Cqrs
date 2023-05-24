using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class FaultingCommandInterceptor : ICommandInterceptor<FaultingCommand>
{
    public Task InterceptAsync(FaultingCommand command, CommandHandlerDelegate<FaultingCommand> next, CancellationToken cancellationToken)
    {
        return next(command, cancellationToken);
    }
}
