using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;
using Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd;
using Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd.Queries;

namespace Wards.Application.UseCases.WardsHashtags
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWardsHashtagsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarWardHashtagUseCase, CriarWardHashtagUseCase>();
            services.AddScoped<ICriarWardHashtagCommand, CriarWardHashtagCommand>();

            services.AddScoped<IListarHashtagQtdUseCase, ListarHashtagQtdUseCase>();
            services.AddScoped<IListarHashtagQtdQuery, ListarHashtagQtdQuery>();

            return services;
        }
    }
}