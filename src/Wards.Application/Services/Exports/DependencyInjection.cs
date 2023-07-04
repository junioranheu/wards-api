using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Exports.XLSX.Exportar;

namespace Wards.Application.Services.Exports
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExportXlsxService(this IServiceCollection services)
        {
            services.AddScoped<IExportXLSXService, ExportXLSXService>();

            return services;
        }
    }
}