using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs.Infrastructure
{
    public interface IInterceptorRegistry
    {
        IEnumerable<object> GetCommandInterceptors(Type commandType);
        IEnumerable<object> GetQueryInterceptors(Type queryType, Type resultType);
    }
}
