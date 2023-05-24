using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class FaultingQueryHandler : IQueryHandler<FaultingQuery, SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(FaultingQuery query, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
