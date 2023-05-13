using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Wards.AtualizarWard;
using Wards.Application.UseCases.Wards.AtualizarWard.Commands;
using Wards.Application.UseCases.Wards.CriarWard;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.DeletarWard;
using Wards.Application.UseCases.Wards.DeletarWard.Commands;
using Wards.Application.UseCases.Wards.ListarWard;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.ObterWard;
using Wards.Application.UseCases.Wards.ObterWard.Queries;

namespace Wards.Application.UseCases.Wards
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWardsApplication(this IServiceCollection services)
        {
            services.AddScoped<IAtualizarWardUseCase, AtualizarWardUseCase>();
            services.AddScoped<IAtualizarWardCommand, AtualizarWardCommand>();

            services.AddScoped<ICriarWardUseCase, CriarWardUseCase>();
            services.AddScoped<ICriarWardCommand, CriarWardCommand>();

            services.AddScoped<IDeletarWardUseCase, DeletarWardUseCase>();
            services.AddScoped<IDeletarWardCommand, DeletarWardCommand>();

            services.AddScoped<IListarWardUseCase, ListarWardUseCase>();
            services.AddScoped<IListarWardQuery, ListarWardQuery>();

            services.AddScoped<IObterWardUseCase, ObterWardUseCase>();
            services.AddScoped<IObterWardQuery, ObterWardQuery>();

            services.AddScoped<IBulkCopyCriarWardUseCase, BulkCopyCriarWardUseCase>();
            services.AddScoped<IBulkCopyCriarWardCommand, BulkCopyCriarWardCommand>();

            return services;
        }
    }
}