using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Filters
{
    public sealed class ErrorFilter : ExceptionFilterAttribute
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
            string mensagemErroCompleta = $"Ocorreu um erro ao processar sua requisição. Data: {ObterDetalhesDataHora()}. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? $"Mais informações: {ex.InnerException.Message}" : $"Mais informações: {ex.Message}")}";
            string mensagemErroSimples = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;

            var detalhes = new BadRequestObjectResult(new
            {
                Codigo = StatusCodes.Status500InternalServerError,
                Data = ObterDetalhesDataHora(),
                Caminho = context.HttpContext.Request.Path,
                Mensagens = new string[] { mensagemErroSimples }
            });

            await CriarLog(context, mensagemErroCompleta, await ObterUsuarioId(context));
            ExibirILogger(ex, mensagemErroCompleta);

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