// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Encapsulates the call to the next wrapper, or eventually the handler, in a command processing pipeline.
    /// </summary>
    /// <returns>An awaitable task representing the asynchronous operation.</returns>
    public delegate Task HandlerDelegate();
}
