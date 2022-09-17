using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleQueryInterceptorWithLowPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithLowPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Low;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithHighestMinusThreePriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithHighestMinusThreePriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Highest - 3;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithHighPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithHighPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.High;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithLowerPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithLowerPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Lower;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithVeryHighPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithVeryHighPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.VeryHigh;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleQueryInterceptorWithHighestPriority : IQueryInterceptor<SampleQuery, SampleQueryResult>
    {
        private readonly Queue<Type> log;
        public SampleQueryInterceptorWithHighestPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Highest;
        public Task<SampleQueryResult> InterceptAsync(SampleQuery query, HandlerDelegate<SampleQueryResult> next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }
}
