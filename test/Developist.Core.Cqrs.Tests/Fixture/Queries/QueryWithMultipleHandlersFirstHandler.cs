namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class QueryWithMultipleHandlersFirstHandler : IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(QueryWithMultipleHandlers query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
