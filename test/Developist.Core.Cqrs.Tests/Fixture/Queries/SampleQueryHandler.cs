using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
{
    private readonly Queue<Type> _log;

    public SampleQueryHandler(Queue<Type> log) => _log = log;

    public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return Task.FromResult(new SampleQueryResult());
    }
}
