using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wards.API.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var excecao = context.Exception;

            var detalhes = new BadRequestObjectResult(new
            {
                Code = StatusCodes.Status500InternalServerError,
                Messages = new string[] { $"Ocorreu um erro ao processar sua requisição. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(excecao.Message) ? $"Mais informações {excecao.Message}" : string.Empty)}" },
            });

            context.Result = detalhes;

            context.ExceptionHandled = true;
        }
    }
}