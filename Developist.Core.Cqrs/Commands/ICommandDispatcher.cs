// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the interface for a command dispatcher.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches the specified command to a single handler for handling.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to dispatch.</typeparam>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="cancellationToken">The cancellation token to oberve.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    }
}
