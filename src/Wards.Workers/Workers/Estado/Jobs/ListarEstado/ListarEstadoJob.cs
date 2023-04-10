using Microsoft.AspNetCore.Http;
using Quartz;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Domain.Entities;
using Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura;

namespace Wards.WorkersServices.Workers.Estado.Jobs.ListarEstado
{
    [DisallowConcurrentExecution]
    public sealed class ListarEstadoJob : IJob, IListarEstadoJob
    {
        private readonly ICriarLogCommand _criarLogCommand;
        private readonly IListarEstadoQuery _listarEstadoQuery;

        public ListarEstadoJob(ICriarLogCommand criarLogCommand, IListarEstadoQuery listarEstadoQuery)
        {
            _criarLogCommand = criarLogCommand;
            _listarEstadoQuery = listarEstadoQuery;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                IEnumerable<Domain.Entities.Estado> listaEstados = await _listarEstadoQuery.Execute();
                await Console.Out.WriteLineAsync($"Foram encontrados {listaEstados.Count()} estados");

                Log log = new() { Descricao = $"Sucesso no Worker {typeof(ObterTemperaturaJob)}", StatusResposta = StatusCodes.Status200OK };
                await _criarLogCommand.Execute(log);
            }
            catch (Exception ex)
            {
                Log log = new() { Descricao = $"Houve um erro no Worker {typeof(ObterTemperaturaJob)}: {ex.Message}", StatusResposta = StatusCodes.Status500InternalServerError };
                await _criarLogCommand.Execute(log);
            }
        }
    }
}