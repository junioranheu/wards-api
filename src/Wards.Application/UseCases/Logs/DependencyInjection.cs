using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.ExportarLog;
using Wards.Application.UseCases.Logs.ExportarLog.Commands;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.ListarLog.Queries;

namespace Wards.Application.UseCases.Logs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLogsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarLogUseCase, CriarLogUseCase>();
            services.AddScoped<ICriarLogCommand, CriarLogCommand>();

            services.AddScoped<IListarLogUseCase, ListarLogUseCase>();
            services.AddScoped<IListarLogQuery, ListarLogQuery>();

            services.AddScoped<IExportarLogUseCase, ExportarLogUseCase>();
            services.AddScoped<IExportarLogCommand, ExportarLogCommand>();

            return services;
        }
    }
}