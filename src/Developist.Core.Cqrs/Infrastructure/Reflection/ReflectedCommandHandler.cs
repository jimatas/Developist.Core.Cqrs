using Developist.Core.Cqrs.Commands;
using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedCommandHandler
    {
        private readonly object _handler;
        private readonly MethodInfo _handleMethod;

        public ReflectedCommandHandler(Type commandType, IHandlerRegistry registry)
        {
            _handler = registry.GetCommandHandler(commandType);
            _handleMethod = _handler.GetType().GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync));
        }

        public Task HandleAsync(ICommand command, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)_handleMethod.Invoke(_handler, new object[] { command, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
