using AutoMapper;
using Quartz;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura
{
    public sealed class ObterTemperaturaJob : IJob, IObterTemperaturaJob
    {
        private readonly IMapper _map;
        private readonly IListarUsuarioQuery _listarQuery;

        //public ObterTemperaturaJob(IMapper map, IListarUsuarioQuery listarQuery)
        //{
        //    _map = map;
        //    _listarQuery = listarQuery;
        //}

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData1 = context.JobDetail.JobDataMap.GetString("jobData1");

            Console.Clear();
            await Console.Out.WriteLineAsync($"{jobData1} - {HorarioBrasilia()}");
        }
    }
}