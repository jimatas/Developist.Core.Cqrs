namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a delegate for handling queries of type <typeparamref name="TQuery"/> and returning results of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
/// <param name="query">The query to handle.</param>
/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
/// <returns>A task representing the asynchronous operation. The task result contains the query result of type <typeparamref name="TResult"/>.</returns>
public delegate Task<TResult> QueryHandlerDelegate<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
    where TQuery : IQuery<TResult>;
