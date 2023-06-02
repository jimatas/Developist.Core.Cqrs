namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryInterceptorWithLowPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
{
    private readonly Queue<Type> _log;

    public SampleQueryInterceptorWithLowPriority(Queue<Type> log) => _log = log;

    public PriorityLevel Priority => PriorityLevel.Low;

    public Task<SampleQueryResult> InterceptAsync(SampleQuery query, QueryHandlerDelegate<SampleQuery, SampleQueryResult> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(query, cancellationToken);
    }
}
