using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    public interface IQueryInterceptor<in TQuery, TResult> : IPrioritizable
        where TQuery : IQuery<TResult>
    {
        Task<TResult> InterceptAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken);
    }
}
