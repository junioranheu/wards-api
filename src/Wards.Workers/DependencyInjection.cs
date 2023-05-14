using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Wards.WorkersServices.Workers.Estado.Jobs.ListarEstado;
using Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura;

namespace Wards.WorkersServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionWorkersServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddQuartz(services, builder);

            return services;
        }

        private static void AddQuartz(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddQuartz(x =>
            {
                x.UseMicrosoftDependencyInjectionJobFactory();

                AddJobs(x, builder);
            });

            services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        }

        private static void AddJobs(IServiceCollectionQuartzConfigurator q, WebApplicationBuilder builder)
        {
            q.AddJobAndTrigger<ListarEstadoJob>(builder.Configuration);
            q.AddJobAndTrigger<ObterTemperaturaJob>(builder.Configuration);
        }
    }

    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration) where T : IJob
        {
            // Usar o nome do IJob como está na key do appsettings.json;
            string jobName = typeof(T).Name;

            // Tentar carregar o schedule;
            string? configKey = $"Quartz:{jobName}";
            string? cronSchedule = configuration[configKey];

            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"Nenhuma configuração de intervalo foi configurada em appsettings.json para o job {jobName}");
            }

            // Registrar o job;
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(x => x.WithIdentity(jobKey));
            quartz.AddTrigger(x => x.ForJob(jobKey).WithIdentity($"{jobName}-trigger").WithCronSchedule(cronSchedule));
        }
    }
}