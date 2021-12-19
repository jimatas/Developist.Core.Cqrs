// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    public interface IQueryDispatcher
    {
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
