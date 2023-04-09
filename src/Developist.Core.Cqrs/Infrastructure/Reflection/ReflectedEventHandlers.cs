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
        private readonly IEnumerable<object> _handlers;
        private readonly MethodInfo _handleMethod;

        public ReflectedEventHandlers(Type eventType, IHandlerRegistry registry)
        {
            _handlers = registry.GetEventHandlers(eventType);
            _handleMethod = typeof(IEventHandler<>)
                .MakeGenericType(eventType)
                .GetMethod(nameof(IEventHandler<IEvent>.HandleAsync));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ReflectedEventHandler> GetEnumerator()
        {
            return _handlers.Select(handler => new ReflectedEventHandler(handler, _handleMethod)).GetEnumerator();
        }
    }
}
