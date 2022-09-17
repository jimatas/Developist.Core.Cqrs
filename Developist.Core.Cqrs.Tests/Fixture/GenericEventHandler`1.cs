using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class GenericEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        private readonly Queue<Type> log;
        public GenericEventHandler(Queue<Type> log) => this.log = log;
        public Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }
}
