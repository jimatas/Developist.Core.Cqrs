namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

[PipelinePriority(PriorityLevel.Highest)]
public class SampleQueryInterceptorWithHighestPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public SampleQueryInterceptorWithHighestPriority(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> InterceptAsync(
        SampleQuery query,
        QueryHandlerDelegate<SampleQuery, SampleQueryResult> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(query, cancellationToken);
    }
}
