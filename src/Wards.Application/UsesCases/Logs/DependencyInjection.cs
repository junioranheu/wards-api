using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Application.UsesCases.Logs.ListarLog;
using Wards.Application.UsesCases.Logs.ListarLog.Queries;

namespace Wards.Application.UsesCases.Logs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLogsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarLogUseCase, CriarLogUseCase>();
            services.AddScoped<ICriarLogCommand, CriarLogCommand>();

            services.AddScoped<IListarLogUseCase, ListarLogUseCase>();
            services.AddScoped<IListarLogQuery, ListarLogQuery>();

            return services;
        }
    }
}
