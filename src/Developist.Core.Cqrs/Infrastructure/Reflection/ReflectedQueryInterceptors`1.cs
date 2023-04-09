using Developist.Core.Cqrs.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedQueryInterceptors<TResult> : IEnumerable<ReflectedQueryInterceptor<TResult>>
    {
        private readonly IEnumerable<object> _interceptors;
        private readonly MethodInfo _interceptMethod;

        public ReflectedQueryInterceptors(Type queryType, IHandlerRegistry registry)
        {
            _interceptors = registry.GetQueryInterceptors(queryType, typeof(TResult));
            _interceptMethod = typeof(IQueryInterceptor<,>)
                .MakeGenericType(queryType, typeof(TResult))
                .GetMethod(nameof(IQueryInterceptor<IQuery<TResult>, TResult>.InterceptAsync));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ReflectedQueryInterceptor<TResult>> GetEnumerator()
        {
            return _interceptors.Select(interceptor => new ReflectedQueryInterceptor<TResult>(interceptor, _interceptMethod)).GetEnumerator();
        }
    }
}
