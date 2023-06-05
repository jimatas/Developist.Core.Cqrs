namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class FaultingCommandHandler : ICommandHandler<FaultingCommand>
{
    public Task HandleAsync(FaultingCommand command, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
