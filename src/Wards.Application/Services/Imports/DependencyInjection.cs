using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Imports.CSV.Importar;

namespace Wards.Application.Services.Imports
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