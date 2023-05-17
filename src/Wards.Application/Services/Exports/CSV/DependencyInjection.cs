using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Exports.CSV.Exportar;

namespace Wards.Application.Services.Exports.CSV
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExportCsvService(this IServiceCollection services)
        {
            services.AddScoped<IExportCsvService, ExportCsvService>();

            return services;
        }
    }
}