using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public delegate Task<TResult> HandlerDelegate<TResult>();
}
