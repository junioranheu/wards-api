using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Domain.Enums;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IMapper _map;
        private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly ICriarUsuarioRoleUseCase _criarUsuarioRoleUseCase;

        public UsuariosController(
            IMapper map,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarUsuarioRoleUseCase criarUsuarioRoleUseCase)
        {
            _map = map;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarUsuarioRoleUseCase = criarUsuarioRoleUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        public async Task<ActionResult<UsuarioOutput>> CriarUsuario(UsuarioInput input)
        {
            UsuarioOutput output = _map.Map<UsuarioOutput>(input);
             = await _criarUsuarioUseCase.Criar(input);

            if (output.Usuarios!.UsuarioId > 0)
                await _criarUsuarioRoleUseCase.Criar(dto.UsuariosRolesId!, usuarioDTO.Usuarios!.UsuarioId);

            return Ok(output);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        public async Task<ActionResult<IEnumerable<UsuarioOutput>>> ListarUsuario()
        {
            var lista = await _listarUsuarioUseCase.Listar();

            if (lista == null)
            {
                return NotFound();
            }

            return Ok(lista);
        }

        [HttpGet]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        public async Task<ActionResult<UsuarioOutput>> ObterUsuario(int id)
        {
            var item = await _obterUsuarioUseCase.Obter(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
