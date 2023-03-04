using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Logs.ListarLog;
using Wards.Domain.Entities;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Log>))]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListarUsuario()
        {
            var resp = await _listarUseCase.Execute();

            if (resp == null)
            {
                return NotFound();
            }

            return Ok(resp);
        }
    }
}
