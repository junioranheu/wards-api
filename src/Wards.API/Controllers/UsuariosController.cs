using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;
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

        public UsuariosController(
            IMapper map,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IObterUsuarioUseCase obterUsuarioUseCase)
        {
            _map = map;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        public async Task<ActionResult<UsuarioDTO>> CriarUsuario(Usuario input)
        {
            UsuarioDTO usuarioDTO = await _criarUsuarioUseCase.Criar(input);

            if (usuarioDTO.Usuarios!.UsuarioId > 0)
                await _criarUsuarioPerfilUseCase.ExecuteAsync(dto.UsuariosRolesId!, usuarioDTO.Usuarios!.UsuarioId);

            return Ok(usuarioDTO);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ListarUsuario()
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
        public async Task<ActionResult<UsuarioDTO>> ObterUsuario(int id)
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
