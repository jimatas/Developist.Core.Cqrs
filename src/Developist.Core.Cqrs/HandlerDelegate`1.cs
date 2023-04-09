using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents a delegate that handles a query asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    /// <returns>A task representing the asynchronous operation, which returns the result of the query.</returns>
    public delegate Task<TResult> HandlerDelegate<TResult>();
}
