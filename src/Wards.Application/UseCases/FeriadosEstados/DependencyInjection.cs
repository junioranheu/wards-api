using Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado;
using Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands;
using Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado;
using Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Wards.Application.UseCases.FeriadosEstados
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFeriadosEstadosApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarFeriadoEstadoUseCase, CriarFeriadoEstadoUseCase>();
            services.AddScoped<ICriarFeriadoEstadoCommand, CriarFeriadoEstadoCommand>();

            services.AddScoped<IDeletarFeriadoEstadoUseCase, DeletarFeriadoEstadoUseCase>();
            services.AddScoped<IDeletarFeriadoEstadoCommand, DeletarFeriadoEstadoCommand>();

            return services;
        }
    }
}