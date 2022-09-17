using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs.Infrastructure
{
    public interface IHandlerRegistry
    {
        object GetCommandHandler(Type commandType);
        object GetQueryHandler(Type queryType, Type resultType);
        IEnumerable<object> GetEventHandlers(Type eventType);
    }
}
