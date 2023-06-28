using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Wards.API.Filters.Base;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.Shared.Input;

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

            int usuarioId = await new BaseFilter().BaseObterUsuarioId(filterContextExecuted);
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