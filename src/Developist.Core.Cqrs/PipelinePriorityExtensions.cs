namespace Developist.Core.Cqrs;

/// <summary>
/// Provides extension methods for determining the priority levels of command and query interceptors in a CQRS pipeline.
/// </summary>
internal static class PipelinePriorityExtensions
{
    /// <summary>
    /// Retrieves the priority level of a command interceptor.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command intercepted.</typeparam>
    /// <param name="interceptor">The command interceptor from which to retrieve the priority level.</param>
    /// <returns>The priority level of the command interceptor. Returns <see cref="PriorityLevel.Normal"/> if no priority is set.</returns>
    public static PriorityLevel GetPriority<TCommand>(this ICommandInterceptor<TCommand> interceptor)
        where TCommand : ICommand
    {
        return interceptor.GetType().GetCustomAttribute<PipelinePriorityAttribute>(inherit: true)?.Priority ?? PriorityLevel.Normal;
    }

    /// <summary>
    /// Retrieves the priority level of a query interceptor.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query intercepted.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    /// <param name="interceptor">The query interceptor from which to retrieve the priority level.</param>
    /// <returns>The priority level of the query interceptor. Returns <see cref="PriorityLevel.Normal"/> if no priority is set.</returns>
    public static PriorityLevel GetPriority<TQuery, TResult>(this IQueryInterceptor<TQuery, TResult> interceptor)
        where TQuery : IQuery<TResult>
    {
        return interceptor.GetType().GetCustomAttribute<PipelinePriorityAttribute>(inherit: true)?.Priority ?? PriorityLevel.Normal;
    }
}
