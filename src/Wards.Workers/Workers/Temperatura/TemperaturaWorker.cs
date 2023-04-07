using Microsoft.AspNetCore.Http;
using Quartz;
using Quartz.Impl;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.WorkersServices.Workers.Temperatura.Jobs;

namespace Wards.WorkersServices.Workers.Temperatura
{
    public sealed class TemperaturaWorker : ITemperaturaWorker
    {
        private readonly ICriarLogUseCase _criarLogUseCase;

        public TemperaturaWorker(ICriarLogUseCase criarLogUseCase)
        {
            _criarLogUseCase = criarLogUseCase;
        }

        public async Task Worker()
        {
            StdSchedulerFactory factory = new();
            IScheduler scheduler = await factory.GetScheduler();

            try
            {
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<HelloJob>().
                                 Build();

                ITrigger trigger = TriggerBuilder.Create().
                                   StartNow().
                                   WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever()).
                                   Build();

                //ITrigger trigger = TriggerBuilder.Create().
                //                   WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(23, 45)).
                //                   Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                LogInput log = new()
                {
                    TipoRequisicao = null,
                    Endpoint = null,
                    Parametros = null,
                    Descricao = $"Houve um erro no Worker {typeof(TemperaturaWorker)}: {ex.Message}",
                    StatusResposta = StatusCodes.Status500InternalServerError,
                    UsuarioId = null
                };

                await _criarLogUseCase.Execute(log);
                await scheduler.Shutdown();
            }
        }
    }
}