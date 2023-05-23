﻿using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    /// <summary>
    /// Represents a delegate used for dispatching queries and returning a result asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    /// <param name="query">The query to be dispatched.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
    internal delegate Task<TResult> QueryDispatcherDelegate<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}