using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class BaseQueryHandler : IQueryHandler<BaseQuery, SampleQueryResult>
{
    private readonly Queue<Type> _log;

    public BaseQueryHandler(Queue<Type> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(BaseQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.FromResult(new SampleQueryResult());
    }
}
