namespace Developist.Core.Cqrs;

/// <summary>
/// Represents a query that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <remarks>
/// This is a marker interface; there are no members to be implemented.
/// </remarks>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQuery<out TResult>
{
}
