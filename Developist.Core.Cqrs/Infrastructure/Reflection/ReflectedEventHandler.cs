using Developist.Core.Cqrs.Events;

using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedEventHandler
    {
        private readonly object handler;
        private readonly MethodInfo handleMethod;

        public ReflectedEventHandler(object handler, MethodInfo handleMethod)
        {
            this.handler = handler;
            this.handleMethod = handleMethod;
        }

        public PriorityLevel Priority => ((IPrioritizable)handler).Priority;

        public Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)handleMethod.Invoke(handler, new object[] { @event, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Throw(exception.InnerException);
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
