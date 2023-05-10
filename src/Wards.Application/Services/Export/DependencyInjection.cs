using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Export.XLSX.Exportar;

namespace Wards.Application.Services.Export
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddXlsxExportService(this IServiceCollection services)
        {
            services.AddScoped<IExportService, ExportService>();

            return services;
        }
    }
}