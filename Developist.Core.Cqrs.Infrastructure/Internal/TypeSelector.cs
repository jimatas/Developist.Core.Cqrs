// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs
{
    internal class TypeSelector
    {
        private readonly IServiceCollection services;
        private readonly IEnumerable<Type> types;
        private readonly Dictionary<Type, ICollection<Type>> typeRegistrations = new();

        public TypeSelector(IServiceCollection services, IEnumerable<Type> types)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public TypeRegistrationSelector AsImplementedInterfaces()
        {
            foreach (var type in types)
            {
                foreach (var iface in type.GetInterfaces().Where(IsRegistrable))
                {
                    if (!typeRegistrations.ContainsKey(iface))
                    {
                        typeRegistrations.Add(iface, new HashSet<Type>());
                    }
                    typeRegistrations[iface].Add(type);
                }
            }

            return new TypeRegistrationSelector(services, typeRegistrations);

            static bool IsRegistrable(Type iface) => !(iface.IsNotPublic || iface.IsNested || IsDisposable(iface));
            static bool IsDisposable(Type iface) => iface == typeof(IDisposable) || iface == typeof(IAsyncDisposable);
        }
    }
}
