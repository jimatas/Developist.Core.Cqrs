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
        private readonly object _handler;
        private readonly MethodInfo _handleMethod;

        public ReflectedQueryHandler(Type queryType, IHandlerRegistry registry)
        {
            _handler = registry.GetQueryHandler(queryType, typeof(TResult));
            _handleMethod = _handler.GetType().GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        }

        public Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            try
            {
                return (Task<TResult>)_handleMethod.Invoke(_handler, new object[] { query, cancellationToken });
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return Task.FromException<TResult>(exception.InnerException);
            }
        }
    }
}
