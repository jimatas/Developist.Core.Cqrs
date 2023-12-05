namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a query dispatcher responsible for dispatching queries and returning results.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches the specified query asynchronously to its corresponding handler and returns the query result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the query.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the query result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
