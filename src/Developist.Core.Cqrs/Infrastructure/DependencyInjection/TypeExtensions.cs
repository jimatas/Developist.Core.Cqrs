using System;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    internal static class TypeExtensions
    {
        public static bool IsConcrete(this Type type) => !(type.IsInterface || type.IsAbstract);

        public static Type[] GetImplementedGenericInterfaces(this Type type, Type genericTypeDefinition)
        {
            return type.FindInterfaces((candidate, criteria) => candidate.IsGenericType && candidate.GetGenericTypeDefinition().Equals(criteria), genericTypeDefinition);
        }
    }
}
