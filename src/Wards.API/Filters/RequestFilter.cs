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

            IEnumerable<UsuarioRole>? usuarioRoles = await GetUsuarioRoles(filterContextExecuted);

            Log log = new()
            {
                TipoRequisicao = request.Method ?? string.Empty,
                Endpoint = request.Path.Value ?? string.Empty,
                Parametros = GetParametrosRequisicao(filterContextExecuting),
                StatusResposta = response.StatusCode > 0 ? response.StatusCode : 0,
                UsuarioRoleId = (usuarioRoles?.Count() > 0 ? usuarioRoles.FirstOrDefault().UsuarioRoleId : 0)
            };

            await _criarLogUseCase.Criar(log);
        }

        private static async Task<IEnumerable<UsuarioRole>?> GetUsuarioRoles(ActionExecutedContext filterContextExecuted)
        {
            var obterUsuarioRoleUseCase = filterContextExecuted.HttpContext.RequestServices.GetService<IObterUsuarioRoleUseCase>();
            IEnumerable<UsuarioRole>? usuarioRoles = await obterUsuarioRoleUseCase.ObterUsuarioRolesByEmailComCache(filterContextExecuted);

            return usuarioRoles;
        }

        private static string GetParametrosRequisicao(ActionExecutingContext filterContextExecuting)
        {
            var parametros = filterContextExecuting.ActionArguments.FirstOrDefault().Value ?? string.Empty;
            string parametrosSerialiazed = !String.IsNullOrEmpty(parametros.ToString()) ? JsonConvert.SerializeObject(parametros) : string.Empty;

            return parametrosSerialiazed;
        }
    }
}
