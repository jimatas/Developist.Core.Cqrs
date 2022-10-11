using Developist.Core.Cqrs.Queries;

using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedQueryHandler<TResult>
    {
        private readonly object handler;
        private readonly MethodInfo handleMethod;

        public ReflectedQueryHandler(Type queryType, IHandlerRegistry registry)
        {
            handler = registry.GetQueryHandler(queryType, typeof(TResult));
            handleMethod = handler.GetType().GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        }

        public Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            try
            {
                return (Task<TResult>)handleMethod.Invoke(handler, new object[] { query, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Throw(exception.InnerException);
                return Task.FromException<TResult>(exception.InnerException);
            }
        }
    }
}
