namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Provides a set of predefined constants that can be used to specify the priority level of interceptors within a pipeline.
    /// </summary>
    /// <remarks>
    /// A lower priority value indicates that the corresponding interceptor should run later in the pipeline, 
    /// while a higher priority value indicates that the interceptor should run earlier in the pipeline.
    /// </remarks>
    public enum PriorityLevel : sbyte
    {
        /// <summary>
        /// Interceptors with this priority level should run first in the pipeline.
        /// </summary>
        Highest = 127,

        /// <summary>
        /// Interceptors with this priority level should run early in the pipeline, after the highest priority interceptors.
        /// </summary>
        VeryHigh = 96,

        /// <summary>
        /// Interceptors with this priority level should run before most other interceptors.
        /// </summary>
        High = 64,

        /// <summary>
        /// Interceptors with this priority level should run after high-priority interceptors but before normal-priority interceptors.
        /// </summary>
        AboveNormal = 32,

        /// <summary>
        /// Interceptors with this priority level should run at a standard position in the pipeline.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Interceptors with this priority level should run after normal priority interceptors, but before low priority interceptors.
        /// </summary>
        BelowNormal = -32,

        /// <summary>
        /// Interceptors with this priority level should run later in the pipeline, after normal and below-normal priority interceptors.
        /// </summary>
        Low = -64,

        /// <summary>
        /// Interceptors with this priority level should run near the end of the pipeline, before the lowest priority interceptors.
        /// </summary>
        VeryLow = -96,

        /// <summary>
        /// Interceptors with this priority level should run last in the pipeline.
        /// </summary>
        Lowest = -128
    }
}
