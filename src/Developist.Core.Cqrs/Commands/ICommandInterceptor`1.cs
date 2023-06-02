using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the contract for an interceptor that intercepts commands of type <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to be intercepted, which must implement the <see cref="ICommand"/> interface.</typeparam>
    public interface ICommandInterceptor<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Intercepts a command asynchronously.
        /// </summary>
        /// <param name="command">The command to be intercepted.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InterceptAsync(TCommand command, CommandHandlerDelegate<TCommand> next, CancellationToken cancellationToken);
    }
}
