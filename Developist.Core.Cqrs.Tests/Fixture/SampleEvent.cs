using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleEvent : IEvent
    {
    }

    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
