using Microsoft.AspNetCore.Http;
using Quartz;
using Quartz.Impl;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura;

namespace Wards.WorkersServices.Workers.Temperatura
{
    public sealed class TemperaturaWorker
    {
        private readonly ICriarLogUseCase _criarLogUseCase;

        public TemperaturaWorker(ICriarLogUseCase criarLogUseCase)
        {
            _criarLogUseCase = criarLogUseCase;
        }

        public async Task Worker()
        {
            LogInput log = new();
            StdSchedulerFactory factory = new();
            IScheduler scheduler = await factory.GetScheduler();

            try
            {
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<ObterTemperaturaJob>().
                                 UsingJobData("jobData1", "Isso é um teste").
                                 Build();

                ITrigger trigger = TriggerBuilder.Create().
                                   StartNow().
                                   WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever()).
                                   Build();

                //ITrigger trigger = TriggerBuilder.Create().
                //                   WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(23, 45)).
                //                   Build();

                await scheduler.ScheduleJob(job, trigger);
                log = new() { Descricao = $"Sucesso {typeof(TemperaturaWorker)}", StatusResposta = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                await scheduler.Shutdown();
                log = new() { Descricao = $"Houve um erro no Worker {typeof(TemperaturaWorker)}: {ex.Message}", StatusResposta = StatusCodes.Status500InternalServerError };
            }
            finally
            {
                await _criarLogUseCase.Execute(log);
            }
        }
    }
}