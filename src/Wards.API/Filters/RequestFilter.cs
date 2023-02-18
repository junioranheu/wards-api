using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole;
using Wards.Domain.Entities;

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
            HttpResponse response = filterContextExecuted.HttpContext.Response;

            var obterUsuarioRoleUseCase = filterContextExecuted.HttpContext.RequestServices.GetService<IObterUsuarioRoleUseCase>();
            IEnumerable<UsuarioRole> usuarioRoles = await obterUsuarioRoleUseCase.ObterCacheObterUsuarioRolesByEmail(GetUsuarioEmail(filterContextExecuted));

            Log l = new()
            {
                TipoRequisicao = request.Method ?? string.Empty,
                Endpoint = request.Path.Value ?? string.Empty,
                Parametros = GetParametrosRequisicao(filterContextExecuting),
                StatusResposta = response.StatusCode > 0 ? response.StatusCode : 0,
                UsuarioRoleId = (usuarioRoles.FirstOrDefault()?.UsuarioRoleId > 0 ? usuarioRoles.FirstOrDefault().UsuarioRoleId : 0)
            };

            await _criarLogUseCase.Criar(l);
        }

        private static string GetParametrosRequisicao(ActionExecutingContext filterContextExecuting)
        {
            var parametros = filterContextExecuting.ActionArguments.FirstOrDefault().Value ?? string.Empty;
            string parametrosSerialiazed = !String.IsNullOrEmpty(parametros.ToString()) ? JsonConvert.SerializeObject(parametros) : string.Empty;

            return parametrosSerialiazed;
        }

        private static string GetUsuarioEmail(ActionExecutedContext filterContextExecuted)
        {
            if (filterContextExecuted.HttpContext.User.Identity.IsAuthenticated)
            {
                var claim = filterContextExecuted.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                return claim.Value ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
