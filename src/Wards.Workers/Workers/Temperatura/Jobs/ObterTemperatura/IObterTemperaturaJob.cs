using Quartz;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura
{
    public interface IObterTemperaturaJob
    {
        Task Execute(IJobExecutionContext context);
    }
}