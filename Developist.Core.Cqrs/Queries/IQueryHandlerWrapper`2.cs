// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    public interface IQueryHandlerWrapper<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken);
    }
}
