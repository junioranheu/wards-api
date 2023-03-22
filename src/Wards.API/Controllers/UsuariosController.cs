using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IListarUsuarioUseCase _listarUseCase;
        private readonly IObterUsuarioUseCase _obterUseCase;

        public UsuariosController(
            IListarUsuarioUseCase listarUseCase,
            IObterUsuarioUseCase obterUseCase)
        {
            _listarUseCase = listarUseCase;
            _obterUseCase = obterUseCase;
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UsuarioOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<IEnumerable<UsuarioOutput>>> Listar()
        {
            var resp = await _listarUseCase.Execute();

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) } });

            return Ok(resp);
        }

        [HttpGet]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> Obter(int id)
        {
            var resp = await _obterUseCase.Execute(id);

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) } });

            return Ok(resp);
        }
    }
}