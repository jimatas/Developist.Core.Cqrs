namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

// This class is deliberately declared as internal to prevent it from being picked up by the default AddHandlersFromAssembly implementation.
internal class PartiallyClosedQueryHandler<TQuery> : IQueryHandler<TQuery, SampleQueryResult>
    where TQuery : IQuery<SampleQueryResult>
{
    public Task<SampleQueryResult> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
