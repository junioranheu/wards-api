using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Wards.CriarWard;
using Wards.Application.UsesCases.Wards.DeletarWard;
using Wards.Application.UsesCases.Wards.ListarWard;
using Wards.Application.UsesCases.Wards.ObterWard;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Application.UsesCases.Wards.Shared.Output;
using Wards.Domain.Enums;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WardsController : Controller
    {
        private readonly IAtualizarWardUseCase _atualizarUseCase;
        private readonly ICriarWardUseCase _criarUseCase;
        private readonly IDeletarWardUseCase _deletarUseCase;
        private readonly IListarWardUseCase _listarUseCase;
        private readonly IObterWardUseCase _obterUseCase;

        public WardsController(
            ICriarWardUseCase criarUseCase,
            IDeletarWardUseCase deletarUseCase,
            IListarWardUseCase listarUseCase,
            IObterWardUseCase obterUseCase)
        {
            _criarUseCase = criarUseCase;
            _deletarUseCase = deletarUseCase;
            _listarUseCase = listarUseCase;
            _obterUseCase = obterUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<ActionResult<int>> Atualizar(WardInput input)
        {
            var resp = await _atualizarUseCase.Execute(input);

            if (resp < 0)
                return BadRequest();

            return Ok(resp);
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<ActionResult<int>> Criar(WardInput input)
        {
            var resp = await _criarUseCase.Execute(input);

            if (resp < 0)
                return BadRequest();

            return Ok(resp);
        }

        [HttpDelete]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Deletar(int id)
        {
            await _deletarUseCase.Execute(id);
            return Ok();
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WardOutput>))]
        public async Task<ActionResult<IEnumerable<WardOutput>>> Listar()
        {
            var resp = await _listarUseCase.Execute();

            if (resp is null)
                return NotFound();

            return Ok(resp);
        }

        [HttpGet]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WardOutput))]
        public async Task<ActionResult<WardOutput>> Obter(int id)
        {
            var resp = await _obterUseCase.Execute(id);

            if (resp is null)
                return NotFound();

            return Ok(resp);
        }
    }
}