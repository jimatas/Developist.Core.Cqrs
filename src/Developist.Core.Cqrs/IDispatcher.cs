namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines a dispatcher that can dispatch commands, events, and queries to their corresponding handlers.
    /// </summary>
    public interface IDispatcher : ICommandDispatcher, IEventDispatcher, IQueryDispatcher
    {
    }
}
