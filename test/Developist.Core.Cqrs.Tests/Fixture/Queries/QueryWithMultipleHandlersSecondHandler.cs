using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class QueryWithMultipleHandlersSecondHandler : IQueryHandler<QueryWithMultipleHandlers, SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(QueryWithMultipleHandlers query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
