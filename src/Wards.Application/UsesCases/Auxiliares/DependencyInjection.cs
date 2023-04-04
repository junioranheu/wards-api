using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Auxiliares.ListarEstado;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;

namespace Wards.Application.UsesCases.Auxiliares
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuxiliaresApplication(this IServiceCollection services)
        {
            services.AddScoped<IListarEstadoUseCase, ListarEstadoUseCase>();
            services.AddScoped<IListarEstadoQuery, ListarEstadoQuery>();

            return services;
        }
    }
}