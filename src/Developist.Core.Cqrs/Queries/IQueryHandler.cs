namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a handler for processing queries of type <typeparamref name="TQuery"/> and returning results of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the specified query asynchronously and returns the result.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the query result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
