using Developist.Core.Cqrs.Commands;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedCommandInterceptor
    {
        private readonly object _interceptor;
        private readonly MethodInfo _interceptMethod;

        public ReflectedCommandInterceptor(object interceptor, MethodInfo interceptMethod)
        {
            _interceptor = interceptor;
            _interceptMethod = interceptMethod;
        }

        public PriorityLevel Priority => (_interceptor as IPrioritizable)?.Priority ?? PriorityLevel.Normal;

        public Task InterceptAsync(ICommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)_interceptMethod.Invoke(_interceptor, new object[] { command, next, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
