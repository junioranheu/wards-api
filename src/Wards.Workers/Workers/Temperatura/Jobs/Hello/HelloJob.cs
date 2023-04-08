using AutoMapper;
using Quartz;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.Hello
{
    public sealed class HelloJob : IJob, IHelloJob
    {
        private readonly IMapper _map;
        private readonly IListarUsuarioQuery _listarQuery;

        public HelloJob(IMapper map, IListarUsuarioQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"Olá! {HorarioBrasilia()}");
        }
    }
}