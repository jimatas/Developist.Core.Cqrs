using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleCommandInterceptorWithHighestPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithHighestPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Highest;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithLowestPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithLowestPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Lowest;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithLowestPlusOnePriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithLowestPlusOnePriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.Lowest + 1;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithNormalPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithNormalPriority(Queue<Type> log) => this.log = log;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithVeryLowPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithVeryLowPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.VeryLow;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }

    public class SampleCommandInterceptorWithVeryHighPriority : ICommandInterceptor<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandInterceptorWithVeryHighPriority(Queue<Type> log) => this.log = log;
        public PriorityLevel Priority => PriorityLevel.VeryHigh;
        public Task InterceptAsync(SampleCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return next();
        }
    }
}
