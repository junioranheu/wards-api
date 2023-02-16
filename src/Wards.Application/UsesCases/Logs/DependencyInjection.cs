using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.CriarLog.Commands;

namespace Wards.Application.UsesCases.Logs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLogsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarLogUseCase, CriarLogUseCase>();
            services.AddScoped<ICriarLogCommand, CriarLogCommand>();

            return services;
        }
    }
}
