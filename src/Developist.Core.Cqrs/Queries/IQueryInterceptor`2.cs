using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the contract for an interceptor that intercepts queries of type <typeparamref name="TQuery"/> and returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to be intercepted, which must implement the <see cref="IQuery{TResult}"/> interface.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    public interface IQueryInterceptor<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Intercepts a query asynchronously and returns the result.
        /// </summary>
        /// <param name="query">The query to be intercepted.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
        Task<TResult> InterceptAsync(TQuery query, QueryHandlerDelegate<TQuery, TResult> next, CancellationToken cancellationToken);
    }
}
