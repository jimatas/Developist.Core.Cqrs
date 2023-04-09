using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Represents a delegate that handles a command asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public delegate Task HandlerDelegate();
}
