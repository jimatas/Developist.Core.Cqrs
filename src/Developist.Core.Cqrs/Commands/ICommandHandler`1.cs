﻿using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    /// <summary>
    /// Defines the contract for a command handler that handles commands of type <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to be handled, which must implement the <see cref="ICommand"/> interface.</typeparam>
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Handles a command asynchronously.
        /// </summary>
        /// <param name="command">The command to be handled.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}