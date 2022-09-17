using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    public interface ICommandInterceptor<in TCommand> : IPrioritizable
        where TCommand : ICommand
    {
        Task InterceptAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken);
    }
}
