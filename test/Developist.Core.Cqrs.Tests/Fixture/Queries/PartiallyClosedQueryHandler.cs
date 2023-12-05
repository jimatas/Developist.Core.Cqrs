namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

// Deliberately declared as internal to prevent AddHandlersFromAssembly from discovering this class.
internal class PartiallyClosedQueryHandler<TQuery> : IQueryHandler<TQuery, SampleQueryResult>
    where TQuery : IQuery<SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        Assert.Fail("We should not be able to get here!");
        return Task.FromResult(new SampleQueryResult());
    }
}
