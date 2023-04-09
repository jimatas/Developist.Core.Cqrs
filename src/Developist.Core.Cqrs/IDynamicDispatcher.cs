using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines a dynamic dispatcher that can dispatch commands, events, and queries to their corresponding handlers without requiring their types to be known at compile time.
    /// </summary>
    public interface IDynamicDispatcher : IDynamicCommandDispatcher, IDynamicEventDispatcher, IDynamicQueryDispatcher
    {
    }
}
