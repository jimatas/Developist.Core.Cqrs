using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand> 
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
