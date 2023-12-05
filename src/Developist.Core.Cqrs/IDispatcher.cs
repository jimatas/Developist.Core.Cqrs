namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a dispatcher interface that combines command, query, and event dispatching capabilities.
/// </summary>
public interface IDispatcher : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
{
}
