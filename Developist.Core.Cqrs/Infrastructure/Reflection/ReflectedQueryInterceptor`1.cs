using Developist.Core.Cqrs.Queries;

using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedQueryInterceptor<TResult>
    {
        private readonly object interceptor;
        private readonly MethodInfo interceptMethod;

        public ReflectedQueryInterceptor(object interceptor, MethodInfo interceptMethod)
        {
            this.interceptor = interceptor;
            this.interceptMethod = interceptMethod;
        }

        public PriorityLevel Priority => ((IPrioritizable)interceptor).Priority;

        public Task<TResult> InterceptAsync(IQuery<TResult> query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            try
            {
                return (Task<TResult>)interceptMethod.Invoke(interceptor, new object[] { query, next, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Throw(exception.InnerException);
                return Task.FromException<TResult>(exception.InnerException);
            }
        }
    }
}
