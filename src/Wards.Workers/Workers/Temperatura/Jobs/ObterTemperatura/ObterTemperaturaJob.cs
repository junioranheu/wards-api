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

        //public HelloJob(IMapper map, IListarUsuarioQuery listarQuery)
        //{
        //    _map = map;
        //    _listarQuery = listarQuery;
        //}

        public ObterTemperaturaJob()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.Clear();
            await Console.Out.WriteLineAsync($"Olá! {HorarioBrasilia()}");
        }
    }
}