using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using static Wards.Utils.Common;

namespace Wards.API.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly ICriarLogUseCase _criarLogUseCase;

        public ErrorFilter(ILogger<ErrorFilter> logger, ICriarLogUseCase criarLogUseCase)
        {
            _logger = logger;
            _criarLogUseCase = criarLogUseCase;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            Exception ex = context.Exception;
            string mensagemErro = $"Ocorreu um erro ao processar sua requisição. Data: {DetalharDataHora()}. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? $"Mais informações: {ex.InnerException.Message}" : $"Mais informações: {ex.Message}")}";

            var detalhes = new BadRequestObjectResult(new
            {
                Code = StatusCodes.Status500InternalServerError,
                Messages = new string[] { mensagemErro }
            });

            await CriarLog(context, mensagemErro, await ObterUsuarioId(context));
            ExibirILogger(ex, mensagemErro);

            context.Result = detalhes;
            context.ExceptionHandled = true;
        }

        private static string ObterUsuarioEmail(ExceptionContext context)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                // Obter o e-mail do usuário pela Azure;
                //var claim = filterContextExecuted.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                //return claim.Value ?? string.Empty;

                // Obter o e-mail do usuário pela autenticação própria;
                string email = context.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }

        private static async Task<int> ObterUsuarioId(ExceptionContext context)
        {
            var service = context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>();
            UsuarioOutput? usuario = await service!.Execute(ObterUsuarioEmail(context));

            return usuario is not null ? usuario.UsuarioId : 0;
        }

        private async Task CriarLog(ExceptionContext context, string mensagemErro, int? usuarioId)
        {
            LogInput log = new()
            {
                TipoRequisicao = context.HttpContext.Request.Method ?? string.Empty,
                Endpoint = context.HttpContext.Request.Path.ToString() ?? string.Empty,
                Parametros = string.Empty,
                Descricao = mensagemErro,
                StatusResposta = StatusCodes.Status500InternalServerError,
                UsuarioId = usuarioId > 0 ? usuarioId : null
            };

            await _criarLogUseCase.Execute(log);
        }

        private void ExibirILogger(Exception ex, string mensagemErro)
        {
            _logger.LogError(ex, "{mensagemErro}", mensagemErro);
        }
    }
}
