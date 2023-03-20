using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Logs.ListarLog;
using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Domain.Enums;

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
        public async Task<ActionResult<IEnumerable<LogOutput>>> Listar()
        {
            var resp = await _listarUseCase.Execute();

            if (resp is null)
                return NotFound();

            return Ok(resp);
        }
    }
}