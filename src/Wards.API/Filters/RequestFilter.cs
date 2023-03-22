using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

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
            ActionExecutedContext filterContextExecuted = await next();
            HttpRequest request = filterContextExecuted.HttpContext.Request;
            // HttpResponse response = filterContextExecuted.HttpContext.Response;
            int? statusResposta = (filterContextExecuted.Result as ObjectResult)?.StatusCode;

            int usuarioId = await ObterUsuarioId(filterContextExecuted);
            string parametros = ObterParametrosRequisicao(filterContextExecuting);

            LogInput log = new()
            {
                TipoRequisicao = request.Method ?? string.Empty,
                Endpoint = request.Path.Value ?? string.Empty,
                Parametros = parametros.Contains("Senha") ? string.Empty : parametros,
                StatusResposta = statusResposta > 0 ? (int)statusResposta : 0,
                UsuarioId = usuarioId > 0 ? usuarioId : null
            };

            await _criarLogUseCase.Execute(log);
        }

        private static string ObterUsuarioEmail(ActionExecutedContext filterContextExecuted)
        {
            if (filterContextExecuted.HttpContext.User.Identity!.IsAuthenticated)
            {
                // Obter o e-mail do usuário pela Azure;
                //var claim = filterContextExecuted.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                //return claim.Value ?? string.Empty;

                // Obter o e-mail do usuário pela autenticação própria;
                string email = filterContextExecuted.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }

        private static async Task<int> ObterUsuarioId(ActionExecutedContext filterContextExecuted)
        {
            var service = filterContextExecuted.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>();
            UsuarioOutput? usuario = await service!.Execute(ObterUsuarioEmail(filterContextExecuted));

            return usuario is not null ? usuario.UsuarioId : 0;
        }

        private static string ObterParametrosRequisicao(ActionExecutingContext filterContextExecuting)
        {
            var parametros = filterContextExecuting.ActionArguments.FirstOrDefault().Value ?? string.Empty;

            try
            {
                string parametrosSerialiazed = !String.IsNullOrEmpty(parametros.ToString()) ? JsonConvert.SerializeObject(parametros) : string.Empty;
                return parametrosSerialiazed;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
