using Microsoft.AspNetCore.Mvc;
using Wards.API.Controllers;
using Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro;
using Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Input;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace NewsLetterCadastros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsLettersCadastrosController : BaseController<NewsLettersCadastrosController>
    {
        private readonly ICriarNewsLetterCadastroUseCase _criarUseCase;
        private readonly IListarNewsLetterCadastroUseCase _listarUseCase;

        public NewsLettersCadastrosController(ICriarNewsLetterCadastroUseCase criarUseCase, IListarNewsLetterCadastroUseCase listarUseCase)
        {
            _criarUseCase = criarUseCase;
            _listarUseCase = listarUseCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(NewsLetterCadastroOutput))]
        public async Task<ActionResult<int>> Criar(NewsLetterCadastroInput input)
        {
            var resp = await _criarUseCase.Execute(input);

            if (resp < 1)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.BadRequest));
            }

            return Ok(resp);
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NewsLetterCadastroOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NewsLetterCadastroOutput))]
        public async Task<ActionResult<IEnumerable<NewsLetterCadastroOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }
    }
}