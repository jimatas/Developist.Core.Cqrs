// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
