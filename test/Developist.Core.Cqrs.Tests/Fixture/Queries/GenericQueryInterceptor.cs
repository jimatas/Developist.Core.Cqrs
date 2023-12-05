namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class GenericQueryInterceptor<TQuery, TResult> : IQueryInterceptor<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly Queue<object> _log;

    public GenericQueryInterceptor(Queue<object> log) => _log = log;

    public Task<TResult> InterceptAsync(
        TQuery query, 
        QueryHandlerDelegate<TQuery, TResult> next, 
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(query, cancellationToken);
    }
}
