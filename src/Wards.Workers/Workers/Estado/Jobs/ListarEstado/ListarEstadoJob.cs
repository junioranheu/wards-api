using Microsoft.AspNetCore.Http;
using Quartz;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

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
                PaginacaoInput pag = new() { IsSelectAll = true };
                IEnumerable<Domain.Entities.Estado> listaEstados = await _listarEstadoQuery.Execute(pag);
                await Console.Out.WriteLineAsync($"Foram encontrados {listaEstados.Count()} estados");

                Log log = new() { Descricao = $"Sucesso no Worker {typeof(ListarEstadoJob)}", StatusResposta = StatusCodes.Status200OK };
                await _criarLogCommand.Execute(log);
            }
            catch (Exception ex)
            {
                Log log = new() { Descricao = $"Houve um erro no Worker {typeof(ListarEstadoJob)}: {ex.Message}", StatusResposta = StatusCodes.Status500InternalServerError };
                await _criarLogCommand.Execute(log);
            }
        }
    }
}