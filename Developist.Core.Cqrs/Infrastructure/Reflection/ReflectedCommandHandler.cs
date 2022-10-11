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
        private readonly object handler;
        private readonly MethodInfo handleMethod;

        public ReflectedCommandHandler(Type commandType, IHandlerRegistry registry)
        {
            handler = registry.GetCommandHandler(commandType);
            handleMethod = handler.GetType().GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync));
        }

        public Task HandleAsync(ICommand command, CancellationToken cancellationToken)
        {
            try
            {
                return (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Throw(exception.InnerException);
                return Task.FromException(exception.InnerException);
            }
        }
    }
}
