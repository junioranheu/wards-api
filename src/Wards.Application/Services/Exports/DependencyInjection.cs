using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Exports.CSV;
using Wards.Application.Services.Exports.XLSX;

namespace Wards.Application.Services.Exports
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExportsService(this IServiceCollection services)
        {
            services.AddScoped<IExportCSVService, ExportCSVService>();

            services.AddScoped<IExportXLSXService, ExportXLSXService>();

            return services;
        }
    }
}