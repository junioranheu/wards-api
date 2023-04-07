using Microsoft.Extensions.DependencyInjection;

namespace Wards.WorkersServices.Workers.Temperatura
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTemperaturaWorkerWS(this IServiceCollection services)
        {
            services.AddScoped<TemperaturaWorker>();

            return services;
        }
    }
}