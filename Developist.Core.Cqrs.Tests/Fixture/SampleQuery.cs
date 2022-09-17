using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleQuery : IQuery<SampleQueryResult>
    {
    }

    public record SampleQueryResult(bool Success = true);

    public class SampleQueryHandler : IQueryHandler<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryHandler(Queue<Type> log) => this.log = log;
        public Task<SampleQueryResult> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.FromResult(new SampleQueryResult());
        }
    }
}
