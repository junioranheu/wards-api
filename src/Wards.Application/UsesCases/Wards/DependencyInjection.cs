using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Wards.CriarWard;
using Wards.Application.UsesCases.Wards.CriarWard.Commands;
using Wards.Application.UsesCases.Wards.DeletarWard;
using Wards.Application.UsesCases.Wards.DeletarWard.Commands;
using Wards.Application.UsesCases.Wards.ListarWard;
using Wards.Application.UsesCases.Wards.ListarWard.Queries;
using Wards.Application.UsesCases.Wards.ObterWard;
using Wards.Application.UsesCases.Wards.ObterWard.Queries;

namespace Wards.Application.UsesCases.Wards
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWardsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarWardUseCase, CriarWardUseCase>();
            services.AddScoped<ICriarWardCommand, CriarWardCommand>();

            services.AddScoped<IDeletarWardUseCase, DeletarWardUseCase>();
            services.AddScoped<IDeletarWardCommand, DeletarWardCommand>();

            services.AddScoped<IListarWardUseCase, ListarWardUseCase>();
            services.AddScoped<IListarWardQuery, ListarWardQuery>();

            services.AddScoped<IObterWardUseCase, ObterWardUseCase>();
            services.AddScoped<IObterWardQuery, ObterWardQuery>();

            return services;
        }
    }
}