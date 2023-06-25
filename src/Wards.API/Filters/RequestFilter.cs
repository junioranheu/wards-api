using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.API.Filters
{
    public sealed class RequestFilter : ActionFilterAttribute
    {
        private readonly ICriarLogUseCase _criarLogUseCase;

        public RequestFilter(ICriarLogUseCase criarLogUseCase)
        {
            _criarLogUseCase = criarLogUseCase;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext filterContextExecuting, ActionExecutionDelegate next)
        {
            if (filterContextExecuting.HttpContext.RequestAborted.IsCancellationRequested)
            {
                filterContextExecuting.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                return;
            }

            ActionExecutedContext filterContextExecuted = await next();
            HttpRequest request = filterContextExecuted.HttpContext.Request;
            int? statusResposta = (filterContextExecuted.Result as ObjectResult)?.StatusCode;

            int usuarioId = await ObterUsuarioId(filterContextExecuted);
            string parametros = ObterParametrosRequisicao(filterContextExecuting);

            LogInput log = new()
            {
                TipoRequisicao = request.Method ?? string.Empty,
                Endpoint = request.Path.Value ?? string.Empty,
                Parametros = parametros.Contains("Senha") ? string.Empty : parametros,
                Descricao = string.Empty,
                StatusResposta = statusResposta > 0 ? (int)statusResposta : StatusCodes.Status500InternalServerError,
                UsuarioId = usuarioId > 0 ? usuarioId : null
            };

            await _criarLogUseCase.Execute(log);
        }

        private static string ObterUsuarioEmail(ActionExecutedContext context)
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

        private static async Task<int> ObterUsuarioId(ActionExecutedContext context)
        {
            var service = context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>();
            UsuarioOutput? usuario = await service!.Execute(ObterUsuarioEmail(context));

            return usuario is not null ? usuario.UsuarioId : 0;
        }

        private static string ObterParametrosRequisicao(ActionExecutingContext filterContextExecuting)
        {
            var parametros = filterContextExecuting.ActionArguments.FirstOrDefault().Value ?? string.Empty;

            try
            {
                string parametrosSerialiazed = !string.IsNullOrEmpty(parametros.ToString()) ? JsonConvert.SerializeObject(parametros) : string.Empty;
                return parametrosSerialiazed;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
