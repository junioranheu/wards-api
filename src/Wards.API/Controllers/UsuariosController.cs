using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<int>> AtualizarUsuario(UsuarioInput input)
        {
            int id = await _atualizarUsuarioUseCase.ExecuteAsync(input);
            return Ok(id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CriarUsuario(UsuarioInput input)
        {
            int id = await _criarUsuarioUseCase.ExecuteAsync(input);
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
