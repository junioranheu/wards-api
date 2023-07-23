using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.AtualizarWard;
using Wards.Application.UseCases.Wards.CriarWard;
using Wards.Application.UseCases.Wards.DeletarWard;
using Wards.Application.UseCases.Wards.ListarWard;
using Wards.Application.UseCases.Wards.ObterWard;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Convert;
using static Wards.Utils.Fixtures.Get;

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
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WardOutput))]
        public async Task<ActionResult<int>> Atualizar(WardInput input)
        {
            input.UsuarioModId = await ObterUsuarioId();
            var resp = await _atualizarUseCase.Execute(input);

            if (resp < 1)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.BadRequest));
            }

            return Ok(resp);
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WardOutput))]
        public async Task<ActionResult<int>> Criar([FromForm] WardInputAlt input, IFormFile formFileImagemPrincipal)
        {
            WardInput w = new()
            {
                Titulo = input.Titulo,
                Conteudo = input.Conteudo
            };

            if (formFileImagemPrincipal is not null)
            {
                w.ImagemPrincipalBlob = await ConverterIFormFileParaBytes(formFileImagemPrincipal);
            }

            w.UsuarioId = await ObterUsuarioId();
            var resp = await _criarUseCase.Execute(w);

            if (resp < 1)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.BadRequest));
            }

            return Ok(resp);
        }

        [HttpDelete]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> Deletar(int id)
        {
            var resp = await _deletarUseCase.Execute(id);
            return Ok(resp);
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WardOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WardOutput))]
        public async Task<ActionResult<IEnumerable<WardOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WardOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WardOutput))]
        public async Task<ActionResult<WardOutput>> Obter(int id)
        {
            var item = await _obterUseCase.Execute(id);

            if (item is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(item);
        }
    }
}