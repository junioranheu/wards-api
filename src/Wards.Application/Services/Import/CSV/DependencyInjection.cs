using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Import.CSV.Importar;

namespace Wards.Application.Services.Import.CSV
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCsvImportService(this IServiceCollection services)
        {
            services.AddScoped<IImportService, ImportService>();

            return services;
        }
    }
}
