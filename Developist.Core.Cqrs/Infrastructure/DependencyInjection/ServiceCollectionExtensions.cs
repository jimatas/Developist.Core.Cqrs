using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsBuilder> buildAction)
        {
            var builder = new CqrsBuilder(services);
            buildAction(builder);
            return services;
        }
    }
}
