using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Queries;

using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    public static class InterceptorRegistryExtensions
    {
        public static IEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>(this IInterceptorRegistry registry)
            where TCommand : ICommand
        {
            return registry.GetCommandInterceptors(typeof(TCommand)).Cast<ICommandInterceptor<TCommand>>();
        }

        public static IEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>(this IInterceptorRegistry registry)
            where TQuery : IQuery<TResult>
        {
            return registry.GetQueryInterceptors(typeof(TQuery), typeof(TResult)).Cast<IQueryInterceptor<TQuery, TResult>>();
        }
    }
}
