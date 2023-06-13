namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class GenericQueryInterceptor<TQuery, TResult> : IQueryInterceptor<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public Task<TResult> InterceptAsync(TQuery query, QueryHandlerDelegate<TQuery, TResult> next, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
