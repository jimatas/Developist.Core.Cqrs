using Developist.Core.Cqrs.Events;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedEventHandler
    {
        private readonly object _handler;
        private readonly MethodInfo _handleMethod;

        public ReflectedEventHandler(object handler, MethodInfo handleMethod)
        {
            _handler = handler;
            _handleMethod = handleMethod;
        }

        public PriorityLevel Priority => ((IPrioritizable)_handler).Priority;

        public Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)_handleMethod.Invoke(_handler, new object[] { @event, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
