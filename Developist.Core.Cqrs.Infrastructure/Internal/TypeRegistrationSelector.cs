// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs
{
    internal class TypeRegistrationSelector
    {
        private readonly IServiceCollection services;
        private readonly IDictionary<Type, ICollection<Type>> typeRegistrations;

        public TypeRegistrationSelector(IServiceCollection services, IDictionary<Type, ICollection<Type>> typeRegistrations)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.typeRegistrations = typeRegistrations ?? throw new ArgumentNullException(nameof(typeRegistrations));
        }

        public IServiceCollection WithLifetime(ServiceLifetime lifetime)
        {
            foreach ((var service, var implementations) in typeRegistrations)
            {
                foreach (var implementation in implementations)
                {
                    services.Add(new ServiceDescriptor(service, implementation, lifetime));
                }
            }

            return services;
        }
    }
}
