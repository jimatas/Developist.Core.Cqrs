using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs
{
    public interface IDynamicDispatcher : IDynamicCommandDispatcher, IDynamicEventDispatcher, IDynamicQueryDispatcher
    {
    }
}
