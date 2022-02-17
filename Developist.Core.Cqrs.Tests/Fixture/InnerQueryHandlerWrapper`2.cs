// Copyright (c) 2022 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Queries;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    public class InnerQueryHandlerWrapper<TQuery, TResult> : IQueryHandlerWrapper<TQuery, TResult>, IPrioritizable
        where TQuery : IQuery<TResult>
    {
        private readonly IList<string> output;
        
        public InnerQueryHandlerWrapper(IList<string> output)
        {
            this.output = output;
        }

        public Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            output.Add($"{nameof(InnerQueryHandlerWrapper<TQuery, TResult>)}.{nameof(HandleAsync)}_Before");

            var taskResult = next();

            output.Add($"{nameof(InnerQueryHandlerWrapper<TQuery, TResult>)}.{nameof(HandleAsync)}_After");

            return taskResult;
        }

        // Should be the last to run, right before the query handler.
        public sbyte Priority => Priorities.Low;
    }
}
