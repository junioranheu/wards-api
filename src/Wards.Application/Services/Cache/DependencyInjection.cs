using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Cache.GenericCache;

namespace Wards.Application.Services.Cache;

public static class DependencyInjection
{
    public static IServiceCollection AddCacheService(this IServiceCollection services)
    {
        services.AddScoped<IGenericCacheService, GenericCacheService>();

        return services;
    }
}