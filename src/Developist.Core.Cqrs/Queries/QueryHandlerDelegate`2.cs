using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents a delegate that handles a query asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to handle, which must implement the <see cref="IQuery{TResult}"/> interface.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
    public delegate Task<TResult> QueryHandlerDelegate<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>;
}
