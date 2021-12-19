// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs.Infrastructure
{
    public interface IHandlerRegistry
    {
        object GetCommandHandler(Type commandType);
        IEnumerable<object> GetCommandHandlerWrappers(Type commandType);
        object GetQueryHandler(Type queryType, Type resultType);
        IEnumerable<object> GetQueryHandlerWrappers(Type queryType, Type resultType);
        IEnumerable<object> GetEventHandlers(Type eventType);
    }
}
