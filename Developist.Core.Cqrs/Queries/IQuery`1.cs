// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Marker interface for a query.
    /// A query is a request for data. It should not make any changes to the system's state.
    /// </summary>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    public interface IQuery<out TResult>
    {
    }
}
