using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    /// <summary>
    /// Defines the contract for a dynamic query dispatcher.
    /// </summary>
    public interface IDynamicQueryDispatcher
    {
        /// <summary>
        /// Dispatches a query asynchronously to its registered handler and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
        /// <param name="query">The query to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
