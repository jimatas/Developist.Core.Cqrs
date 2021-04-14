// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Allows an implementor to define additional behavior around the handling of a query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being wrapped.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    public interface IQueryHandlerWrapper<in TQuery, TResult> : ISortable where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Performs any supplemental work before and/or after query handling.
        /// </summary>
        /// <param name="query">The query to handle.</param>
        /// <param name="next">An awaitable delegate representing the call to the next wrapper or, eventually the handler.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the query result.</returns>
        Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken);
    }
}
