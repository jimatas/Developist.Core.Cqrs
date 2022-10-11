using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs.Infrastructure
{
    public interface IHandlerRegistry
    {
        object GetCommandHandler(Type commandType);
        IEnumerable<object> GetCommandInterceptors(Type commandType);

        IEnumerable<object> GetEventHandlers(Type eventType);

        object GetQueryHandler(Type queryType, Type resultType);
        IEnumerable<object> GetQueryInterceptors(Type queryType, Type resultType);
    }
}
