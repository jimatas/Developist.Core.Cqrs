using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
