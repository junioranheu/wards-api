using Wards.Application.UseCases.CriarFeriados.AtualizarFeriado;
using Wards.Application.UseCases.CriarFeriados.CriarFeriado;
using Wards.Application.UseCases.Feriados.AtualizarFeriado;
using Wards.Application.UseCases.Feriados.AtualizarFeriado.Commands;
using Wards.Application.UseCases.Feriados.CriarFeriado;
using Wards.Application.UseCases.Feriados.CriarFeriado.Commands;
using Wards.Application.UseCases.Feriados.ListarFeriado;
using Wards.Application.UseCases.Feriados.ListarFeriado.Queries;
using Wards.Application.UseCases.Feriados.ObterFeriado;
using Wards.Application.UseCases.Feriados.ObterFeriado.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Wards.Application.UseCases.Feriados
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFeriadosApplication(this IServiceCollection services)
        {
            services.AddScoped<IAtualizarFeriadoUseCase, AtualizarFeriadoUseCase>();
            services.AddScoped<IAtualizarFeriadoCommand, AtualizarFeriadoCommand>();

            services.AddScoped<ICriarFeriadoUseCase, CriarFeriadoUseCase>();
            services.AddScoped<ICriarFeriadoCommand, CriarFeriadoCommand>();

            services.AddScoped<IListarFeriadoUseCase, ListarFeriadoUseCase>();
            services.AddScoped<IListarFeriadoQuery, ListarFeriadoQuery>();

            services.AddScoped<IObterFeriadoUseCase, ObterFeriadoUseCase>();
            services.AddScoped<IObterFeriadoQuery, ObterFeriadoQuery>();

            return services;
        }
    }
}