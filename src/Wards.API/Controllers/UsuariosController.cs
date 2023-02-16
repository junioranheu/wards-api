using Microsoft.AspNetCore.Mvc;
using Wards.Application.UsesCases.Usuarios.AtualizarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

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
            IObterUsuarioUseCase obterUsuarioUseCase
            )
        {
            _atualizarUsuarioUseCase = atualizarUsuarioUseCase;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
        }

        [HttpPut]
        public async Task<ActionResult<int>> AtualizarUsuario(UsuarioDTO dto)
        {
            int id = await _atualizarUsuarioUseCase.ExecuteAsync(dto);
            return Ok(id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CriarUsuario(UsuarioDTO dto)
        {
            int id = await _criarUsuarioUseCase.ExecuteAsync(dto);
            return Ok(id);
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Usuario>>> ListarUsuario()
        {
            var lista = await _listarUsuarioUseCase.ExecuteAsync();

            if (lista == null)
            {
                return NotFound();
            }

            return Ok(lista);
        }

        [HttpGet]
        public async Task<ActionResult<Usuario>> ObterUsuario(int id)
        {
            var item = await _obterUsuarioUseCase.ExecuteAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
