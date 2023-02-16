using Microsoft.AspNetCore.Mvc.Filters;

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
            string tipoRequisicao = request.Method;

            if (tipoRequisicao != "GET")
            {
                var obterUsuarioUseCase = filterContextExecuted.HttpContext.RequestServices.GetService<IObterUsuarioUseCase>();
                int[] usuarioPerfilLista = await obterUsuarioUseCase.GetListaIdUsuarioPerfil(GetUsuarioEmail(filterContextExecuted));

                Log l = new()
                {
                    TipoRequisicao = tipoRequisicao,
                    Endpoint = request.Path.Value ?? string.Empty,
                    Parametros = GetParametrosRequisicao(filterContextExecuting),
                    StatusResposta = response.StatusCode > 0 ? response.StatusCode : 0,
                    IdUsuarioPerfil = usuarioPerfilLista.FirstOrDefault()
                };

                await _criarLogUseCase.ExecuteAsync(l);
            }
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
