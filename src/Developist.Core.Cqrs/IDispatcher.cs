using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines a dispatcher that can dispatch commands, events, and queries to their corresponding handlers.
    /// </summary>
    public interface IDispatcher : ICommandDispatcher, IEventDispatcher, IQueryDispatcher
    {
    }
}
