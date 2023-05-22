using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    /// <summary>
    /// Defines the contract for a query handler that handles queries of type <typeparamref name="TQuery"/> and returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to be handled, which must implement the <see cref="IQuery{TResult}"/> interface.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles a query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The query to be handled.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
