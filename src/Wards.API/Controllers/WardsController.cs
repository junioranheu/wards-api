using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Wards.AtualizarWard;
using Wards.Application.UsesCases.Wards.CriarWard;
using Wards.Application.UsesCases.Wards.DeletarWard;
using Wards.Application.UsesCases.Wards.ListarWard;
using Wards.Application.UsesCases.Wards.ObterWard;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Application.UsesCases.Wards.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WardsController : BaseController<WardsController>
    {
        private readonly IAtualizarWardUseCase _atualizarUseCase;
        private readonly ICriarWardUseCase _criarUseCase;
        private readonly IDeletarWardUseCase _deletarUseCase;
        private readonly IListarWardUseCase _listarUseCase;
        private readonly IObterWardUseCase _obterUseCase;

        public WardsController(
            IAtualizarWardUseCase atualizarUseCase,
            ICriarWardUseCase criarUseCase,
            IDeletarWardUseCase deletarUseCase,
            IListarWardUseCase listarUseCase,
            IObterWardUseCase obterUseCase)
        {
            _atualizarUseCase = atualizarUseCase;
            _criarUseCase = criarUseCase;
            _deletarUseCase = deletarUseCase;
            _listarUseCase = listarUseCase;
            _obterUseCase = obterUseCase;
        }

        [HttpPut]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WardOutput))]
        public async Task<ActionResult<int>> Atualizar(WardInput input)
        {
            input.UsuarioModId = await ObterUsuarioId();
            var resp = await _atualizarUseCase.Execute(input);

            if (resp < 1)
                return StatusCode(StatusCodes.Status400BadRequest, new WardOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.BadRequest) } });

            return Ok(resp);
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WardOutput))]
        public async Task<ActionResult<int>> Criar(WardInput input)
        {
            input.UsuarioId = await ObterUsuarioId();
            var resp = await _criarUseCase.Execute(input);

            if (resp < 1)
                return StatusCode(StatusCodes.Status400BadRequest, new WardOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.BadRequest) } });

            return Ok(resp);
        }

        [HttpDelete]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> Deletar(int id)
        {
            var resp = await _deletarUseCase.Execute(id);
            return Ok(resp);
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WardOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WardOutput))]
        public async Task<ActionResult<IEnumerable<WardOutput>>> Listar()
        {
            var resp = await _listarUseCase.Execute();

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new WardOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });

            return Ok(resp);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WardOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WardOutput))]
        public async Task<ActionResult<WardOutput>> Obter(int id)
        {
            var resp = await _obterUseCase.Execute(id);

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new WardOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });

            return Ok(resp);
        }
    }
}