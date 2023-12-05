namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class FaultingSampleCommandHandler : ICommandHandler<SampleCommand>
{
    public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
