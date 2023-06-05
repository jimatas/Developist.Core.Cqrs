namespace Developist.Core.Cqrs
{
    /// <summary>
    /// Defines the contract for a query that returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
    public interface IQuery<out TResult>
    {
    }
}
