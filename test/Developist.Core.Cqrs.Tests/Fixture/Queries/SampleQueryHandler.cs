namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public SampleQueryHandler(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.FromResult(new SampleQueryResult());
    }
}
