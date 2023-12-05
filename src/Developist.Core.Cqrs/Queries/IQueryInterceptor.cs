namespace Developist.Core.Cqrs;

/// <summary>
/// Represents an interceptor for queries of type <typeparamref name="TQuery"/> that return results of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query to intercept.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryInterceptor<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Intercepts the specified query asynchronously and provides the option to invoke the next component in the pipeline.
    /// </summary>
    /// <param name="query">The query to intercept.</param>
    /// <param name="next">A delegate representing the next query interceptor in the pipeline, or ultimately the query handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the query result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> InterceptAsync(TQuery query, QueryHandlerDelegate<TQuery, TResult> next, CancellationToken cancellationToken);
}
