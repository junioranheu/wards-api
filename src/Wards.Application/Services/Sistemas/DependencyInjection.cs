using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Sistemas.ResetarBancoDados;
using Wards.Application.Services.Sistemas.ResetarBancoDados.Commands;

namespace Wards.Application.Services.Sistemas
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddResetarBancoDadosService(this IServiceCollection services)
        {
            services.AddScoped<IMigrateDatabaseService, MigrateDatabaseService>();
            services.AddScoped<IMigrateDatabaseCommand, MigrateDatabaseCommand>();

            return services;
        }
    }
}