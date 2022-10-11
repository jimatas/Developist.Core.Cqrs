using Developist.Core.Cqrs.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedCommandInterceptors : IEnumerable<ReflectedCommandInterceptor>
    {
        private readonly IEnumerable<object> interceptors;
        private readonly MethodInfo interceptMethod;

        public ReflectedCommandInterceptors(Type commandType, IHandlerRegistry registry)
        {
            interceptors = registry.GetCommandInterceptors(commandType);
            interceptMethod = typeof(ICommandInterceptor<>)
                .MakeGenericType(commandType)
                .GetMethod(nameof(ICommandInterceptor<ICommand>.InterceptAsync));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ReflectedCommandInterceptor> GetEnumerator()
        {
            return interceptors.Select(interceptor => new ReflectedCommandInterceptor(interceptor, interceptMethod)).GetEnumerator();
        }
    }
}
