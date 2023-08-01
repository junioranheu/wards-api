using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Hashtags.ListarHashtag;
using Wards.Application.UseCases.Hashtags.ListarHashtag.Queries;

namespace Wards.Application.UseCases.Hashtags
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHashtagsApplication(this IServiceCollection services)
        {
            services.AddScoped<IListarHashtagUseCase, ListarHashtagUseCase>();
            services.AddScoped<IListarHashtagQuery, ListarHashtagQuery>();

            return services;
        }
    }
}