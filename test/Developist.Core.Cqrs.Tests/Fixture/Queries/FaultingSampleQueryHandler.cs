namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class FaultingSampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
    {
        throw new ApplicationException("There was an error.");
    }
}
