namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class DerivedQueryHandler : IQueryHandler<DerivedQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public DerivedQueryHandler(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(DerivedQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.FromResult(new SampleQueryResult());
    }
}
