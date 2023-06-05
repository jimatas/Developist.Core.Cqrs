namespace System
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/> objects.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is a concrete type (not an interface or abstract class).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a concrete type; otherwise, <c>false</c>.</returns>
        public static bool IsConcrete(this Type type) => !(type.IsInterface || type.IsAbstract);

        /// <summary>
        /// Gets an array of implemented generic interfaces that match the specified generic type definition.
        /// </summary>
        /// <param name="type">The type to retrieve the implemented interfaces from.</param>
        /// <param name="genericTypeDefinition">The generic type definition to match.</param>
        /// <returns>An array of implemented generic interfaces that match the specified generic type definition.</returns>
        public static Type[] GetImplementedGenericInterfaces(this Type type, Type genericTypeDefinition)
        {
            return type.FindInterfaces((candidate, criteria) => candidate.IsGenericType && candidate.GetGenericTypeDefinition().Equals(criteria), genericTypeDefinition);
        }
    }
}
