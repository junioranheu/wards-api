using Quartz;
using static Wards.Utils.Common;

namespace Wards.Workers.TemperaturaWorker.Jobs
{
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"Olá! {HorarioBrasilia()}");
        }
    }
}