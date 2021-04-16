// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public static class EventDispatcherExtensions
    {
        /// <summary>
        /// Disptaches a sequence of events to multiple handlers for handling.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to dispatch from.</param>
        /// <param name="events">The events to dispatach.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        public static async Task DispatchAllAsync(this IEventDispatcher dispatcher, IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            if (events is null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            ICollection<Exception> exceptions = null;

            foreach (var e in events)
            {
                try
                {
                    await dispatcher.DispatchAsync((dynamic)e, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    (exceptions ??= new List<Exception>()).Add(exception);
                }
            }

            if (exceptions is not null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
