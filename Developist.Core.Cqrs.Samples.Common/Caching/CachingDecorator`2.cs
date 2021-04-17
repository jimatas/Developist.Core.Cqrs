// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Samples.Common.Caching
{
    /// <summary>
    /// Adds caching behavior to the query processing pipeline for queries that support it.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query whose result can be cached.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    public class CachingDecorator<TQuery, TResult> : IQueryHandlerWrapper<TQuery, TResult> where TQuery : ICacheableQuery<TResult>
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CachingDecorator<TQuery, TResult>> logger;

        public CachingDecorator(IMemoryCache memoryCache, ILogger<CachingDecorator<TQuery, TResult>> logger)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            if (memoryCache.TryGetValue<TResult>(query.CacheKey, out var result))
            {
                logger.LogDebug("Query result was found in cache. Returning the cached copy.");
                return result;
            }

            logger.LogDebug("Query result was not found in cache. Querying for fresh data to cache.");
            result = await next();
            memoryCache.Set(query.CacheKey, result, absoluteExpirationRelativeToNow: query.CacheDuration);

            return result;
        }
    }
}
