// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Allows an implementor to define additional behavior around the handling of a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being wrapped.</typeparam>
    public interface ICommandHandlerWrapper<in TCommand> : ISortable where TCommand : ICommand
    {
        /// <summary>
        /// Performs any supplemental work before and/or after the command is handled.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        /// <param name="next">An awaitable delegate representing the call to the next wrapper, or eventually the handler.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task HandleAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken);
    }
}
