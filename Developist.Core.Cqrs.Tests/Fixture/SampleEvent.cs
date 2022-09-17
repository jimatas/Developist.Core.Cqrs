using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleEvent : IEvent
    {
    }

    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        private readonly Queue<Type> log;
        public SampleEventHandler(Queue<Type> log) => this.log = log;
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }
}
