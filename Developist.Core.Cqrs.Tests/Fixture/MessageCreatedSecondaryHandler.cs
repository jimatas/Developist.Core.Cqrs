// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Events;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class MessageCreatedSecondaryHandler : IEventHandler<MessageCreated>
    {
        private readonly IList<string> output;

        public MessageCreatedSecondaryHandler(IList<string> output = null)
        {
            this.output = output ?? new List<string>();
        }

        public Task HandleAsync(MessageCreated @event, CancellationToken cancellationToken)
        {
            output.Add($"{nameof(MessageCreatedSecondaryHandler)}.{nameof(HandleAsync)}");

            return Task.CompletedTask;
        }
    }
}
