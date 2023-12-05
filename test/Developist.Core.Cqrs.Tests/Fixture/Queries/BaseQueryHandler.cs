namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class BaseQueryHandler : IQueryHandler<BaseQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public BaseQueryHandler(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(BaseQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return Task.FromResult(new SampleQueryResult());
    }
}
