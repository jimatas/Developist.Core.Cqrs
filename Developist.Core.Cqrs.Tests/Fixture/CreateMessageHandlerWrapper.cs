// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class CreateMessageHandlerWrapper : ICommandHandlerWrapper<CreateMessage>
    {
        private readonly IList<string> output;

        public CreateMessageHandlerWrapper(IList<string> output = null)
        {
            this.output = output ?? new List<string>();
        }

        public Task HandleAsync(CreateMessage command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            output.Add($"{nameof(CreateMessageHandlerWrapper)}.{nameof(HandleAsync)}_Before");

            var taskResult = next();

            output.Add($"{nameof(CreateMessageHandlerWrapper)}.{nameof(HandleAsync)}_After");

            return taskResult;
        }
    }
}
