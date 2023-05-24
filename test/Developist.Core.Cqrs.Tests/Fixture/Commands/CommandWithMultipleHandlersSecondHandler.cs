using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class CommandWithMultipleHandlersSecondHandler : ICommandHandler<CommandWithMultipleHandlers>
{
    public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
