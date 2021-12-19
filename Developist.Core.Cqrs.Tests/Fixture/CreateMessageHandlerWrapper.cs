// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class CreateMessageHandlerWrapper : ICommandHandlerWrapper<CreateMessage>, IPrioritizable
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

        // Lowest priority in the execution pipeline, should run last.
        public sbyte Priority => Priorities.Lower;
    }
}
