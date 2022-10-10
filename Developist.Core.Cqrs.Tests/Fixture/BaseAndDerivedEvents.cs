using Developist.Core.Cqrs.Events;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class BaseEvent : IEvent
    {
    }

    public class DerivedEvent : BaseEvent
    {
    }

    public class BaseEventHandler : IEventHandler<BaseEvent>
    {
        private readonly Queue<Type> log;
        public BaseEventHandler(Queue<Type> log) => this.log = log;
        public Task HandleAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }

    public class DerivedEventHandler : IEventHandler<DerivedEvent>
    {
        private readonly Queue<Type> log;
        public DerivedEventHandler(Queue<Type> log) => this.log = log;
        public Task HandleAsync(DerivedEvent @event, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }
}
