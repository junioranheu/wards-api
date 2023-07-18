using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro;
using Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro.Commands;
using Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro;
using Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro.Queries;

namespace NewsletterCadastros.Application.UseCases.NewslettersCadastros
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNewslettersCadastrosApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarNewsletterCadastroUseCase, CriarNewsletterCadastroUseCase>();
            services.AddScoped<ICriarNewsletterCadastroCommand, CriarNewsletterCadastroCommand>();

            services.AddScoped<IListarNewsletterCadastroUseCase, ListarNewsletterCadastroUseCase>();
            services.AddScoped<IListarNewsletterCadastroQuery, ListarNewsletterCadastroQuery>();

            return services;
        }
    }
}