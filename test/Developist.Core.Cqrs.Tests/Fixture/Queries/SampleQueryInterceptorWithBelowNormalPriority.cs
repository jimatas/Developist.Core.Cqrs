namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

[PipelinePriority(PriorityLevel.BelowNormal)]
public class SampleQueryInterceptorWithBelowNormalPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public SampleQueryInterceptorWithBelowNormalPriority(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> InterceptAsync(
        SampleQuery query,
        QueryHandlerDelegate<SampleQuery, SampleQueryResult> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(query, cancellationToken);
    }
}
