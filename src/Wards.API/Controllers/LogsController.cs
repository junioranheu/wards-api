using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.ExportarXlsxLog;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : BaseController<LogsController>
    {
        private readonly ICriarLogUseCase _criarUseCase;
        private readonly IListarLogUseCase _listarUseCase;
        private readonly IExportarXlsxLogUseCase _exportarXlsxUseCase;

        public LogsController(
            ICriarLogUseCase criarUseCase, 
            IListarLogUseCase listarUseCase,
            IExportarXlsxLogUseCase exportarXlsxUseCase)
        {
            _criarUseCase = criarUseCase;
            _listarUseCase = listarUseCase;
            _exportarXlsxUseCase = exportarXlsxUseCase;
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(bool))]
        public async Task<ActionResult<bool>> Criar(LogInput input)
        {
            await _criarUseCase.Execute(input);
            return Ok(true);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LogOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(LogOutput))]
        public async Task<ActionResult<IEnumerable<LogOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                return StatusCode(StatusCodes.Status404NotFound, new LogOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });
            }

            return Ok(lista);
        }

        [HttpGet("exportarXlsx")]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFound))]
        public async Task<ActionResult> ExportarXLSXCargaGlobal(bool isTodos)
        {
            int usuarioId = await ObterUsuarioId();
            byte[]? xlsx = await _exportarXlsxUseCase.ExecuteAsync(usuarioId, isTodos);

            if (xlsx is null || xlsx.Length == 0)
                return NotFound();

            return File(xlsx, ObterDescricaoEnum(TipoExtensaoArquivoRetorno.XLSX), "export.xlsx");
        }
    }
}