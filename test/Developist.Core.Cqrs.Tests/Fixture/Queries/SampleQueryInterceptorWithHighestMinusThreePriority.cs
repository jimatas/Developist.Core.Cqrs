namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryInterceptorWithHighestMinusThreePriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
{
    private readonly Queue<Type> _log;

    public SampleQueryInterceptorWithHighestMinusThreePriority(Queue<Type> log) => _log = log;

    public PriorityLevel Priority => PriorityLevel.Highest - 3;

    public Task<SampleQueryResult> InterceptAsync(SampleQuery query, QueryHandlerDelegate<SampleQuery, SampleQueryResult> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(query, cancellationToken);
    }
}
