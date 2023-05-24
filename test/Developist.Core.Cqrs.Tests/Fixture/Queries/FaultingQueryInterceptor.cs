using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class FaultingQueryInterceptor : IQueryInterceptor<FaultingQuery, SampleQueryResult>
{
    public Task<SampleQueryResult> InterceptAsync(FaultingQuery query, QueryHandlerDelegate<FaultingQuery, SampleQueryResult> next, CancellationToken cancellationToken)
    {
        return next(query, cancellationToken);
    }
}
