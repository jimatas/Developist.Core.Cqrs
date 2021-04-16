// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the interface for an event handler.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Dispatches the specified event to multiple handlers for handling.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to dispatch.</typeparam>
        /// <param name="event">The event to dispatch.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
    }
}
