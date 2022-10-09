using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public delegate Task DispatcherDelegate<in T>(T arg, CancellationToken cancellationToken = default);
}
