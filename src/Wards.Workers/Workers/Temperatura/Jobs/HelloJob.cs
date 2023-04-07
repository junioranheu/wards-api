using Microsoft.AspNetCore.Http;
using Quartz;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs
{
    public class HelloJob : IJob
    {
        private readonly ICriarLogUseCase _criarLogUseCase;

        public HelloJob(ICriarLogUseCase criarLogUseCase)
        {
            _criarLogUseCase = criarLogUseCase;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            string msg = $"Olá! {HorarioBrasilia()}";
            await Console.Out.WriteLineAsync(msg);

            LogInput log = new()
            {
                TipoRequisicao = null,
                Endpoint = null,
                Parametros = msg,
                Descricao = $"Sucesso {typeof(HelloJob)}",
                StatusResposta = StatusCodes.Status200OK,
                UsuarioId = null
            };

            await _criarLogUseCase.Execute(log);
        }
    }
}