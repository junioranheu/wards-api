using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;

namespace Wards.Application.UseCases.WardsHashtags
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWardsHashtagsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarWardHashtagUseCase, CriarWardHashtagUseCase>();
            services.AddScoped<ICriarWardHashtagCommand, CriarWardHashtagCommand>();

            return services;
        }
    }
}