// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class ExceptionThrowingMessageCreatedHandler : IEventHandler<MessageCreated>
    {
        private readonly IList<string> output;

        public ExceptionThrowingMessageCreatedHandler(IList<string> output)
        {
            this.output = output;
        }

        public static bool ThrowException { get; set; }

        public Task HandleAsync(MessageCreated e, CancellationToken cancellationToken)
        {
            if (ThrowException)
            {
                throw new Exception("This exception was deliberately thrown.");
            }
            output.Add($"{nameof(ExceptionThrowingMessageCreatedHandler)}.{nameof(HandleAsync)}");

            return Task.CompletedTask;
        }
    }
}
