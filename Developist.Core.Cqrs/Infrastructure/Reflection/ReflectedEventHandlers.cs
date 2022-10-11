using Developist.Core.Cqrs.Events;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs.Infrastructure.Reflection
{
    internal class ReflectedEventHandlers : IEnumerable<ReflectedEventHandler>
    {
        private readonly IEnumerable<object> handlers;
        private readonly MethodInfo handleMethod;

        public ReflectedEventHandlers(Type eventType, IHandlerRegistry registry)
        {
            handlers = registry.GetEventHandlers(eventType);
            handleMethod = typeof(IEventHandler<>)
                .MakeGenericType(eventType)
                .GetMethod(nameof(IEventHandler<IEvent>.HandleAsync));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ReflectedEventHandler> GetEnumerator()
        {
            return handlers.Select(handler => new ReflectedEventHandler(handler, handleMethod)).GetEnumerator();
        }
    }
}
