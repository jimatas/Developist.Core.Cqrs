using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    public interface IDynamicQueryDispatcher
    {
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
