namespace Developist.Core.Cqrs;

/// <summary>
/// Enumeration of priority levels for interceptors within a pipeline.
/// </summary>
public enum PriorityLevel : sbyte
{
    /// <summary>
    /// The highest priority level.
    /// Interceptors with this priority level run first in the pipeline.
    /// </summary>
    Highest = 127,

    /// <summary>
    /// A very high priority level.
    /// Interceptors with this priority level run after the <see cref="Highest"/> ones but before the <see cref="High"/> ones.
    /// </summary>
    VeryHigh = 96,

    /// <summary>
    /// A high priority level.
    /// Interceptors with this priority level run after the <see cref="VeryHigh"/> ones but before the <see cref="AboveNormal"/> ones.
    /// </summary>
    High = 64,

    /// <summary>
    /// An above-normal priority level.
    /// Interceptors with this priority level run after the <see cref="High"/> ones but before the <see cref="Normal"/> ones.
    /// </summary>
    AboveNormal = 32,

    /// <summary>
    /// The normal priority level. This is the default priority level and is used when no specific priority is specified.
    /// Interceptors with <see cref="Normal"/> priority run after all higher-priority interceptors.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// A below-normal priority level.
    /// Interceptors with this priority level run after the <see cref="Normal"/> ones but before the <see cref="Low"/> ones.
    /// </summary>
    BelowNormal = -32,

    /// <summary>
    /// A low priority level.
    /// Interceptors with this priority level run after the <see cref="BelowNormal"/> ones but before the <see cref="VeryLow"/> ones.
    /// </summary>
    Low = -64,

    /// <summary>
    /// A very low priority level.
    /// Interceptors with this priority level run after the <see cref="Low"/> ones but before the <see cref="Lowest"/> ones.
    /// </summary>
    VeryLow = -96,

    /// <summary>
    /// The lowest priority level.
    /// Interceptors with this priority level run last in the pipeline.
    /// </summary>
    Lowest = -128
}
