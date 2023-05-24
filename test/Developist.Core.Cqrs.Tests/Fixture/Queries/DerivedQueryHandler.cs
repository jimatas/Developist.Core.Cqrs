using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class DerivedQueryHandler : IQueryHandler<DerivedQuery, SampleQueryResult>
{
    private readonly Queue<Type> _log;

    public DerivedQueryHandler(Queue<Type> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(DerivedQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.FromResult(new SampleQueryResult());
    }
}
