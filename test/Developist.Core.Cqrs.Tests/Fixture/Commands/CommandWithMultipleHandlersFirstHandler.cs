namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class CommandWithMultipleHandlersFirstHandler : ICommandHandler<CommandWithMultipleHandlers>
{
    public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
