using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro.Commands;
using Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro;
using Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro.Queries;

namespace NewsLetterCadastros.Application.UseCases.NewsLettersCadastros
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNewsLettersCadastrosApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarNewsLetterCadastroUseCase, CriarNewsLetterCadastroUseCase>();
            services.AddScoped<ICriarNewsLetterCadastroCommand, CriarNewsLetterCadastroCommand>();

            services.AddScoped<IListarNewsLetterCadastroUseCase, ListarNewsLetterCadastroUseCase>();
            services.AddScoped<IListarNewsLetterCadastroQuery, ListarNewsLetterCadastroQuery>();

            return services;
        }
    }
}