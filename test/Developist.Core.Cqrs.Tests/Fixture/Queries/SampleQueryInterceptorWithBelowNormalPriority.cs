namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryInterceptorWithBelowNormalPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>, IPrioritizable
{
    private readonly Queue<Type> _log;

    public SampleQueryInterceptorWithBelowNormalPriority(Queue<Type> log) => _log = log;

    public PriorityLevel Priority => PriorityLevel.BelowNormal;

    public Task<SampleQueryResult> InterceptAsync(SampleQuery query, QueryHandlerDelegate<SampleQuery, SampleQueryResult> next, CancellationToken cancellationToken)
    {
        _log.Enqueue(GetType());
        return next(query, cancellationToken);
    }
}
