using Microsoft.Extensions.DependencyInjection;

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