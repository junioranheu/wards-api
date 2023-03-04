using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly ICriarUsuarioRoleUseCase _criarUsuarioRoleUseCase;

        public UsuariosController(
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarUsuarioRoleUseCase criarUsuarioRoleUseCase)
        {
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarUsuarioRoleUseCase = criarUsuarioRoleUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<ActionResult<int>> CriarUsuario(UsuarioInput input)
        {
            Usuario usuario = new()
            {
                NomeCompleto = input.Usuarios!.NomeCompleto,
                NomeUsuarioSistema = input.Usuarios!.NomeUsuarioSistema,
                Email = input.Usuarios!.Email,
                Senha = input.Usuarios!.Senha,
                Chamado = input.Usuarios!.Chamado,
                HistPerfisAtivos = input.Usuarios!.HistPerfisAtivos?.Length > 0 ? string.Join(", ", input.Usuarios!.HistPerfisAtivos) : string.Empty
            };

            int usuarioId = await _criarUsuarioUseCase.Execute(usuario);

            if (input.Usuarios!.UsuarioId > 0)
                await _criarUsuarioRoleUseCase.Execute(input.UsuariosRolesId!, usuarioId);

            return Ok(usuarioId);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Usuario>))]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListarUsuario()
        {
            var resp = await _listarUsuarioUseCase.Execute();

            if (resp == null)
            {
                return NotFound();
            }

            return Ok(resp);
        }

        [HttpGet]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        public async Task<ActionResult<Usuario>> ObterUsuario(int id)
        {
            var resp = await _obterUsuarioUseCase.Execute(id);

            if (resp == null)
            {
                return NotFound();
            }

            return Ok(resp);
        }
    }
}
