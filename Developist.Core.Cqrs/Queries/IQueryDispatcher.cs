using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    public interface IQueryDispatcher
    {
        Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;
    }
}
