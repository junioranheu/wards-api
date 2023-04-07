using Quartz;
using Quartz.Impl;
using Wards.Workers.TemperaturaWorker.Jobs;

namespace Wards.Workers.TemperaturaWorker
{
    public sealed class TemperaturaWorker
    {
        public static async Task Worker()
        {
            StdSchedulerFactory factory = new();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<HelloJob>().
                             Build();

            ITrigger trigger = TriggerBuilder.Create().
                               StartNow().
                               WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever()).
                               Build();

            await scheduler.ScheduleJob(job, trigger);
            // await scheduler.Shutdown();
        }
    }
}