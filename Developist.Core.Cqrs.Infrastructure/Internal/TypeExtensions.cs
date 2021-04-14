// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;

namespace Developist.Core.Cqrs
{
    internal static class TypeExtensions
    {
        public static bool IsConcrete(this Type type) => type.IsClass && !type.IsAbstract;
        public static bool IsOpenGeneric(this Type type) => type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        public static Type[] FindGenericInterfaces(this Type type, Type genericTypeDefinition)
            => type.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericTypeDefinition).ToArray();
    }
}
