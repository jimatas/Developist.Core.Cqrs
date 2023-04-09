using Developist.Core.Cqrs.Queries;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedQueryInterceptor<TResult>
    {
        private readonly object _interceptor;
        private readonly MethodInfo _interceptMethod;

        public ReflectedQueryInterceptor(object interceptor, MethodInfo interceptMethod)
        {
            _interceptor = interceptor;
            _interceptMethod = interceptMethod;
        }

        public PriorityLevel Priority => (_interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal;

        public Task<TResult> InterceptAsync(IQuery<TResult> query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            try
            {
                return (Task<TResult>)_interceptMethod.Invoke(_interceptor, new object[] { query, next, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return Task.FromException<TResult>(exception.InnerException);
            }
        }
    }
}
