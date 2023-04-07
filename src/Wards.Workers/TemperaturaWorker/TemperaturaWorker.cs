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
            catch (Exception)
            {
                await scheduler.Shutdown();
            }
        }
    }
}