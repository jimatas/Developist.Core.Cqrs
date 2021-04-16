// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class InnerCommandHandlerWrapper<TCommand> : ICommandHandlerWrapper<TCommand> where TCommand : ICommand
    {
        private readonly IList<string> output;

        public InnerCommandHandlerWrapper(IList<string> output)
        {
            this.output = output;
        }

        public Task HandleAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            output.Add($"{nameof(InnerCommandHandlerWrapper<TCommand>)}.{nameof(HandleAsync)}_Before");

            var taskResult = next();

            output.Add($"{nameof(InnerCommandHandlerWrapper<TCommand>)}.{nameof(HandleAsync)}_After");

            return taskResult;
        }

        int ISortable.SortOrder => 20;
    }
}
