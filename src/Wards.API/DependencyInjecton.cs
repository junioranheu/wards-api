using Wards.API.Filters;

namespace Wards.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(o => o.Filters.Add<RequestFilter>());
            builder.Services.AddControllers(o => o.Filters.Add<ErrorFilter>());
            builder.Services.AddMemoryCache();

            return services;
        }
    }
}
