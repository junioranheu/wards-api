using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.API.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        private readonly ICriarLogUseCase _criarLogUseCase;

        public ErrorFilter(ICriarLogUseCase criarLogUseCase)
        {
            _criarLogUseCase = criarLogUseCase;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var excecao = context.Exception;

            string mensagemErro = $"Ocorreu um erro ao processar sua requisição. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(excecao.InnerException?.Message) ? $"Mais informações: {excecao.InnerException.Message}" : $"Mais informações: {excecao.Message}")}";

            var detalhes = new BadRequestObjectResult(new
            {
                Code = StatusCodes.Status500InternalServerError,
                Messages = new string[] { mensagemErro },
            });

            int usuarioId = await ObterUsuarioId(context);

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
    }
}
