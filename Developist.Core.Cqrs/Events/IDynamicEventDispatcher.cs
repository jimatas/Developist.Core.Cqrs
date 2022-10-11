using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    public interface IDynamicEventDispatcher
    {
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
