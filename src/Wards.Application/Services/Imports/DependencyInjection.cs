using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Imports.CSV.Importar;

namespace Wards.Application.Services.Imports
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddImportCsvService(this IServiceCollection services)
        {
            services.AddScoped<IImportCsvService, ImportCsvService>();

            return services;
        }
    }
}