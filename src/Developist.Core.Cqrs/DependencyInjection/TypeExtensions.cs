namespace System;

/// <summary>
/// Provides extension methods for working with <see cref="Type"/> objects.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="Type"/> is a concrete type (not an interface or abstract class).
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns><see langword="true"/> if the type is concrete; otherwise, <see langword="false"/>.</returns>
    public static bool IsConcrete(this Type type)
    {
        return !type.IsInterface && !type.IsAbstract;
    }

    /// <summary>
    /// Gets an array of <see cref="Type"/> objects representing interfaces implemented by the current type that match the specified generic type definition.
    /// </summary>
    /// <param name="type">The current <see cref="Type"/> to analyze.</param>
    /// <param name="genericTypeDefinition">The generic type definition to match.</param>
    /// <returns>An array of matching interface types.</returns>
    public static Type[] GetImplementedGenericInterfaces(this Type type, Type genericTypeDefinition)
    {
        return type.FindInterfaces(
            filter: (candidate, criteria) => candidate.IsGenericType && candidate.GetGenericTypeDefinition().Equals(criteria),
            filterCriteria: genericTypeDefinition);
    }
}
