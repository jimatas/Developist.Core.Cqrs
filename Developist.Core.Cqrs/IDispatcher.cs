// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines a class that can dispatch instances of all the message types.
    /// </summary>
    public interface IDispatcher : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
    {
    }
}
