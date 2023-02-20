using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.AtualizarUsuario;
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
        private readonly IAtualizarUsuarioUseCase _atualizarUsuarioUseCase;
        private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;

        public UsuariosController(
            IAtualizarUsuarioUseCase atualizarUsuarioUseCase,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IObterUsuarioUseCase obterUsuarioUseCase)
        {
            _atualizarUsuarioUseCase = atualizarUsuarioUseCase;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
        }

        [HttpPut]
        public async Task<ActionResult<int>> AtualizarUsuario(UsuarioDTO dto)
        {
            int id = await _atualizarUsuarioUseCase.Atualizar(dto);
            return Ok(id);
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
