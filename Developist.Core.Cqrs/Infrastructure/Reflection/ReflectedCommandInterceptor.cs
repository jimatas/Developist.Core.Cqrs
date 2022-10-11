using Developist.Core.Cqrs.Commands;

using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedCommandInterceptor
    {
        private readonly object interceptor;
        private readonly MethodInfo interceptMethod;

        public ReflectedCommandInterceptor(object interceptor, MethodInfo interceptMethod)
        {
            this.interceptor = interceptor;
            this.interceptMethod = interceptMethod;
        }

        public PriorityLevel Priority => ((IPrioritizable)interceptor).Priority;

        public Task InterceptAsync(ICommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)interceptMethod.Invoke(interceptor, new object[] { command, next, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Throw(exception.InnerException);
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
