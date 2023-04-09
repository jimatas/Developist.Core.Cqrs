namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Allows for prioritization of interceptors within a pipeline.
    /// </summary>
    public interface IPrioritizable
    {
        /// <summary>
        /// Gets the priority level of the interceptor.
        /// </summary>
        /// <remarks>
        /// The priority level determines the order in which interceptors are executed within a pipeline. 
        /// Higher priority levels will be executed before lower priority levels. 
        /// The default priority level is <see cref="PriorityLevel.Normal"/>.
        /// </remarks>
        PriorityLevel Priority { get; }
    }
}
