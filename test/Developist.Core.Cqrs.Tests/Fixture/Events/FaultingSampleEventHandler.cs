namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class FaultingSampleEventHandler : IEventHandler<SampleEvent>
{
    public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
