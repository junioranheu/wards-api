using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Auxiliares.ListarEstado;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Queries;

namespace Wards.Application.UseCases.Auxiliares
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