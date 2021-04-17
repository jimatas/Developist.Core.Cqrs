// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Cqrs.Samples.Common.Caching
{
    /// <summary>
    /// Implemented by <see cref="IQuery{TResult}"/> implementors whose result can be cached.
    /// </summary>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    public interface ICacheableQuery<TResult> : IQuery<TResult>
    {
        /// <summary>
        /// Unique key by which to identify the query result in cache.
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// The duration for which to keep the query result in cache.
        /// </summary>
        /// <value>Default value is 1 minute.</value>
        TimeSpan CacheDuration => TimeSpan.FromMinutes(1);
    }
}
