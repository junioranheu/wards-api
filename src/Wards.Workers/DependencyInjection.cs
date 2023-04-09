using Microsoft.Extensions.DependencyInjection;
using Wards.WorkersServices.Workers.Temperatura;

namespace Wards.WorkersServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionWorkersServices(this IServiceCollection services)
        {
            services.AddScoped<TemperaturaWorker>();

            return services;
        }
    }
}