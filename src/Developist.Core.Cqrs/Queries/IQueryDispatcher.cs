using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    /// <summary>
    /// Defines the contract for a query dispatcher.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatches a query asynchronously to its registered handler and returns the result.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query to be dispatched, which must implement the <see cref="IQuery{TResult}"/> interface.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
        /// <param name="query">The query to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
        Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;
    }
}
