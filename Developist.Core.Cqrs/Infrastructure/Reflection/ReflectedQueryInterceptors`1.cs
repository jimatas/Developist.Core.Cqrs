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
        private readonly IEnumerable<object> interceptors;
        private readonly MethodInfo interceptMethod;

        public ReflectedQueryInterceptors(Type queryType, IHandlerRegistry registry)
        {
            interceptors = registry.GetQueryInterceptors(queryType, typeof(TResult));
            interceptMethod = typeof(IQueryInterceptor<,>)
                .MakeGenericType(queryType, typeof(TResult))
                .GetMethod(nameof(IQueryInterceptor<IQuery<TResult>, TResult>.InterceptAsync));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ReflectedQueryInterceptor<TResult>> GetEnumerator()
        {
            return interceptors.Select(interceptor => new ReflectedQueryInterceptor<TResult>(interceptor, interceptMethod)).GetEnumerator();
        }
    }
}
