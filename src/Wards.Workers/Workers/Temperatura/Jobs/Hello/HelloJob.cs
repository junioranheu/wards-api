using Quartz;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.Hello
{
    public sealed class HelloJob : IJob, IHelloJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"Olá! {HorarioBrasilia()}");
        }
    }
}