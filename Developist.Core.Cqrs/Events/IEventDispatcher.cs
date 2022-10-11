using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    public interface IEventDispatcher : IDynamicEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
