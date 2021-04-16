// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class GetMessageByIdHandlerWrapper : IQueryHandlerWrapper<GetMessageById, Message>
    {
        private readonly IList<string> output;

        public GetMessageByIdHandlerWrapper(IList<string> output = null)
        {
            this.output = output ?? new List<string>();
        }

        public Task<Message> HandleAsync(GetMessageById query, HandlerDelegate<Message> next, CancellationToken cancellationToken)
        {
            output.Add($"{nameof(GetMessageByIdHandlerWrapper)}.{nameof(HandleAsync)}_Before");

            var taskResult = next();

            output.Add($"{nameof(GetMessageByIdHandlerWrapper)}.{nameof(HandleAsync)}_After");

            return taskResult;
        }
    }
}
