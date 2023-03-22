using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Logs.ListarLog;
using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : Controller
    {
        private readonly IListarLogUseCase _listarUseCase;

        public LogsController(IListarLogUseCase listarUseCase)
        {
            _listarUseCase = listarUseCase;
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LogOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFound))]
        public async Task<ActionResult<IEnumerable<LogOutput>>> Listar(int pagina = 0, int tamanhoPagina = 10)
        {
            var resp = await _listarUseCase.Execute(pagina, tamanhoPagina);

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new LogOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) } });

            return Ok(resp);
        }
    }
}