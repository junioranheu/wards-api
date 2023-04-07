using Quartz;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.Hello
{
    public interface IHelloJob
    {
        Task Execute(IJobExecutionContext context);
    }
}