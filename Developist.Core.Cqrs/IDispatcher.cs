// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs
{
    public interface IDispatcher : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
    {
    }
}
