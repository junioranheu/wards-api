using Microsoft.AspNetCore.Mvc;
using Wards.API.Controllers;
using Wards.API.Filters;
using Wards.Application.Services.Imports.CSV;
using Wards.Application.Services.Imports.Shared.Models.Input;
using Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro;
using Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro;
using Wards.Application.UseCases.NewslettersCadastros.Shared.Input;
using Wards.Application.UseCases.NewslettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Consts;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace NewsletterCadastros.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewslettersController : BaseController<NewslettersController>
    {
        private readonly ICriarNewsletterCadastroUseCase _criarUseCase;
        private readonly IListarNewsletterCadastroUseCase _listarUseCase;
        private readonly IImportCSVService _importCSVService;

        public NewslettersController(
            ICriarNewsletterCadastroUseCase criarUseCase,
            IListarNewsletterCadastroUseCase listarUseCase,
            IImportCSVService importCSVService)
        {
            _criarUseCase = criarUseCase;
            _listarUseCase = listarUseCase;
            _importCSVService = importCSVService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(NewsletterCadastroOutput))]
        public async Task<ActionResult<int>> Criar(NewsletterCadastroInput input)
        {
            var resp = await _criarUseCase.Execute(input);

            if (resp < 1)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.BadRequest));
            }

            return Ok(resp);
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NewsletterCadastroOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NewsletterCadastroOutput))]
        public async Task<ActionResult<IEnumerable<NewsletterCadastroOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpPost("importar")]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [RequestSizeLimit(SistemaConst.QtdLimiteMBsImport)]
        public async Task<ActionResult> Importar([FromForm] ImportCSVInput input)
        {
            if (input.FormFile is null || !input.FormFile!.FileName.EndsWith(".csv"))
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.ArquivoImportFormatoInvalido));
            }

            input.UsuarioId = await ObterUsuarioId();
            input.ClasseAlvo = new NewsletterCadastro();
            input.NomeDaTabelaAlvoParaBulkInsert = "NewslettersCadastros";

            await _importCSVService.ImportarCSV(input);

            return Ok();
        }
    }
}