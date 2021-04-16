// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the interface for an event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of event handled.</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Handles the specified event.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
