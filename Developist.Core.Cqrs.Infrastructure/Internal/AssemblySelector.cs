// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Cqrs
{
    internal class AssemblySelector
    {
        private readonly IServiceCollection services;
        private readonly IEnumerable<Assembly> assemblies;

        public AssemblySelector(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public TypeSelector AddClasses(Func<Type, bool> predicate = null)
        {
            var types = assemblies.SelectMany(assembly => assembly.ExportedTypes)
                .Where(CanBeAdded)
                .Where(predicate ?? (_ => true));

            return new TypeSelector(services, types);

            static bool CanBeAdded(Type type) => type.IsClass && !(type.IsAbstract || type.IsGenericType);
        }
    }
}
