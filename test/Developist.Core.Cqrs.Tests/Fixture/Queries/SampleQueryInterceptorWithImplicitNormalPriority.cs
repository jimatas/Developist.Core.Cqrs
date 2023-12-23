namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public class SampleQueryInterceptorWithImplicitNormalPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
{
    private readonly Queue<object> _log;

    public SampleQueryInterceptorWithImplicitNormalPriority(Queue<object> log) => _log = log;

    public Task<SampleQueryResult> InterceptAsync(
        SampleQuery query,
        QueryHandlerDelegate<SampleQuery, SampleQueryResult> next,
        CancellationToken cancellationToken)
    {
        _log.Enqueue(this);
        return next(query, cancellationToken);
    }
}
