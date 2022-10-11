using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    public interface IDynamicCommandDispatcher
    {
        Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
