using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public delegate Task<TResult> DispatcherDelegate<in T, TResult>(T arg, CancellationToken cancellationToken = default);
}
