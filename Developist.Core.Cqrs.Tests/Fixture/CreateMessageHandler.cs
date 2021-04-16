// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class CreateMessageHandler : ICommandHandler<CreateMessage>
    {
        private readonly IList<string> output;

        public CreateMessageHandler(IList<string> output = null)
        {
            this.output = output ?? new List<string>();
        }

        public Task HandleAsync(CreateMessage command, CancellationToken cancellationToken)
        {
            var newMessage = new Message(command.Id) { Text = command.Text };

            output.Add($"{nameof(CreateMessageHandler)}.{nameof(HandleAsync)}");

            return Task.CompletedTask;
        }
    }
}
