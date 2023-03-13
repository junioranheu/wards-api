using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Wards.API.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var excecao = context.Exception;

            var detalhes = new ProblemDetails
            {
                Title = "Ocorreu um erro ao processar sua requisição",
                Detail = excecao.Message,
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(detalhes);

            context.ExceptionHandled = true;
        }
    }
}