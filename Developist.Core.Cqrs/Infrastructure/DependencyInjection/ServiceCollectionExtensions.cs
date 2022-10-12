using Developist.Core.Cqrs.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Cqrs.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsBuilder> setupAction)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => setupAction);

            var builder = new CqrsBuilder(services);
            setupAction(builder);
            return services;
        }
    }
}
