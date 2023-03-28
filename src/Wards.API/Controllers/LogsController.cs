using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Logs.CriarLog;
using Wards.Application.UsesCases.Logs.ListarLog;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : Controller
    {
        private readonly ICriarLogUseCase _criarUseCase;
        private readonly IListarLogUseCase _listarUseCase;

        public LogsController(ICriarLogUseCase criarUseCase, IListarLogUseCase listarUseCase)
        {
            _criarUseCase = criarUseCase;
            _listarUseCase = listarUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(bool))]
        public async Task<ActionResult<bool>> Criar(LogInput input)
        {
            await _criarUseCase.Execute(input);
            return Ok(true);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LogOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(LogOutput))]
        public async Task<ActionResult<IEnumerable<LogOutput>>> Listar(int pagina = 0, int tamanhoPagina = 10)
        {
            var resp = await _listarUseCase.Execute(pagina, tamanhoPagina);

            if (resp is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new LogOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });
            }

            return Ok(resp);
        }
    }
}