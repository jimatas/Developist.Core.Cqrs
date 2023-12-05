namespace Developist.Core.Cqrs;

/// <summary>
/// Attribute for indicating the priority of interceptors within a pipeline.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public sealed class PipelinePriorityAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinePriorityAttribute"/> class with the specified priority.
    /// </summary>
    /// <param name="priority">The priority level of the interceptor.</param>
    public PipelinePriorityAttribute(PriorityLevel priority) => Priority = priority;

    /// <summary>
    /// Gets the priority level of the interceptor.
    /// </summary>
    /// <remarks>
    /// The priority level determines the order in which interceptors are executed in a pipeline.
    /// Interceptors with higher priority values run earlier in the pipeline,
    /// while those with lower priority values run later.
    /// </remarks>
    public PriorityLevel Priority { get; }
}
