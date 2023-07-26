using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;
using Wards.Application.UseCases.WardsHashtags.ListarWardHashtag;
using Wards.Application.UseCases.WardsHashtags.ListarWardHashtag.Queries;

namespace Wards.Application.UseCases.WardsHashtags
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWardsHashtagsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarWardHashtagUseCase, CriarWardHashtagUseCase>();
            services.AddScoped<ICriarWardHashtagCommand, CriarWardHashtagCommand>();

            services.AddScoped<IListarWardHashtagUseCase, ListarWardHashtagUseCase>();
            services.AddScoped<IListarWardHashtagQuery, ListarWardHashtagQuery>();

            return services;
        }
    }
}