// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Inherited by the <see cref="ICommandHandlerWrapper{TCommand}"/> and <see cref="IQueryHandlerWrapper{TQuery, TResult}"/> interfaces 
    /// to allow for a relative ordering of their instances within an execution pipeline.
    /// </summary>
    public interface ISortable
    {
        /// <summary>
        /// A value that indicates the relative order in which this instance will be executed in the pipeline.
        /// By default the ordering will be from lowest to highest.
        /// </summary>
        /// <value>The default implementation returns 0.</value>
        int SortOrder => default;
    }
}
