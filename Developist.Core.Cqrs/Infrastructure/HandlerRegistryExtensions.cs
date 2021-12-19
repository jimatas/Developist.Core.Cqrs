// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Infrastructure
{
    public static class HandlerRegistryExtensions
    {
        public static ICommandHandler<TCommand> GetCommandHandler<TCommand>(this IHandlerRegistry handlerRegistry)
            where TCommand : ICommand => (ICommandHandler<TCommand>)handlerRegistry.GetCommandHandler(typeof(TCommand));

        public static IEnumerable<ICommandHandlerWrapper<TCommand>> GetCommandHandlerWrappers<TCommand>(this IHandlerRegistry handlerRegistry)
            where TCommand : ICommand => handlerRegistry.GetCommandHandlerWrappers(typeof(TCommand)).Cast<ICommandHandlerWrapper<TCommand>>();

        public static IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>(this IHandlerRegistry handlerRegistry)
            where TQuery : IQuery<TResult> => (IQueryHandler<TQuery, TResult>)handlerRegistry.GetQueryHandler(typeof(TQuery), typeof(TResult));

        public static IEnumerable<IQueryHandlerWrapper<TQuery, TResult>> GetQueryHandlerWrappers<TQuery, TResult>(this IHandlerRegistry handlerRegistry)
            where TQuery : IQuery<TResult> => handlerRegistry.GetQueryHandlerWrappers(typeof(TQuery), typeof(TResult)).Cast<IQueryHandlerWrapper<TQuery, TResult>>();

        public static IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>(this IHandlerRegistry handlerRegistry)
            where TEvent : IEvent => handlerRegistry.GetEventHandlers(typeof(TEvent)).Cast<IEventHandler<TEvent>>();
    }
}
