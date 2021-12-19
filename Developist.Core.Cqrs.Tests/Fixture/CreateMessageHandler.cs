// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class CreateMessageHandler : ICommandHandler<CreateMessage>
    {
        private readonly IDictionary<Guid, Message> database;
        private readonly IList<string> output;

        public CreateMessageHandler(IDictionary<Guid, Message> database, IList<string> output = null)
        {
            this.database = database;
            this.output = output ?? new List<string>();
        }

        public Task HandleAsync(CreateMessage command, CancellationToken cancellationToken)
        {
            var newMessage = new Message(command.Id) { Text = command.Text };
            database.Add(newMessage.Id, newMessage);

            output.Add($"{nameof(CreateMessageHandler)}.{nameof(HandleAsync)}");

            return Task.CompletedTask;
        }
    }
}
