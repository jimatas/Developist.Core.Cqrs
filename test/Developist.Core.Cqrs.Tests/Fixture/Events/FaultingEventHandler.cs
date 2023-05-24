using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture.Events;

public class FaultingEventHandler : IEventHandler<SampleEvent>
{
    public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
