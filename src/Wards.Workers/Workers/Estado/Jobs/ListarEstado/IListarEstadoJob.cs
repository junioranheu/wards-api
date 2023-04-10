using Quartz;

namespace Wards.WorkersServices.Workers.Estado.Jobs.ListarEstado
{
    public interface IListarEstadoJob
    {
        Task Execute(IJobExecutionContext context);
    }
}