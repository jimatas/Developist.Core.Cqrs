namespace Developist.Core.Cqrs;

/// <summary>
/// Represents an interceptor for commands of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to intercept.</typeparam>
public interface ICommandInterceptor<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Intercepts the specified command asynchronously and provides the option to invoke the next component in the pipeline.
    /// </summary>
    /// <param name="command">The command to intercept.</param>
    /// <param name="next">A delegate representing the next command interceptor in the pipeline, or ultimately the command handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InterceptAsync(TCommand command, CommandHandlerDelegate<TCommand> next, CancellationToken cancellationToken);
}
