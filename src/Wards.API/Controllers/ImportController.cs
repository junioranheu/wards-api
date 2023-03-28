using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wards.API.Filters;
using Wards.Application.Services.Import.CSV.Shared;
using Wards.Application.UsesCases.Imports.CriarExemploUsuario;
using Wards.Domain.Consts;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : BaseController<ImportController>
    {
        private readonly ICriarExemploUsuarioUseCase _criarExemploUsuarioUseCase;

        public ImportController(ICriarExemploUsuarioUseCase criarExemploUsuarioUseCase)
        {
            _criarExemploUsuarioUseCase = criarExemploUsuarioUseCase;
        }

        [HttpPost]
        [AuthorizeFilter]
        [RequestSizeLimit(SistemaConst.QtdLimiteMBsImport)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<ActionResult<string>?> CriarExemploUsuario([FromForm] ImportInput importInput)
        {
            if (!importInput.FormFile!.FileName.EndsWith(".csv"))
            {
                return BadRequest();
            }

            // int justificativaId = await _criarJustificativaUseCase.Execute(importInput.Descricao!, await ObterUsuarioId());
            int justificativaId = 1;
            var resultados = await _criarExemploUsuarioUseCase.Execute(importInput.FormFile!, justificativaId);

            if (resultados.Item2 || resultados.Item1?.Rows.Count > 0)
            {
                // await _deletarJustificativaUseCase.Execute(justificativaId);
                return StatusCode(StatusCodes.Status400BadRequest, (resultados.Item1?.Rows.Count > 0 ? JsonConvert.SerializeObject(resultados.Item1) : ObterDescricaoEnum(CodigoErroEnum.ErroInterno)));
            }

            return Ok(string.Empty);
        }
    }
}