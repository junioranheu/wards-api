using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UseCases.CriarFeriados.AtualizarFeriado;
using Wards.Application.UseCases.CriarFeriados.CriarFeriado;
using Wards.Application.UseCases.Feriados.ListarFeriado;
using Wards.Application.UseCases.Feriados.ObterFeriado;
using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData;
using Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData;
using Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado;
using Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado;
using Wards.Application.UseCases.Feriados.Shared.Models.Input;
using Wards.Application.UseCases.Feriados.Shared.Models.Output;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeriadosController : BaseController<FeriadosController>
    {
        private readonly IAtualizarFeriadoUseCase _atualizarFeriadoUseCase;
        private readonly ICriarFeriadoUseCase _criarFeriadoUseCase;
        private readonly IListarFeriadoUseCase _listarFeriadoUseCase;
        private readonly IObterFeriadoUseCase _obterFeriadoUseCase;
        private readonly ICriarFeriadoDataUseCase _criarFeriadoDataUseCase;
        private readonly IDeletarFeriadoDataUseCase _deletarFeriadoDataUseCase;
        private readonly ICriarFeriadoEstadoUseCase _criarFeriadoEstadoUseCase;
        private readonly IDeletarFeriadoEstadoUseCase _deletarFeriadoEstadoUseCase;

        public FeriadosController(
            IAtualizarFeriadoUseCase atualizarFeriadoUseCase,
            ICriarFeriadoUseCase criarFeriadoUseCase,
            IListarFeriadoUseCase listarFeriadoUseCase,
            IObterFeriadoUseCase obterFeriadoUseCase,
            ICriarFeriadoDataUseCase criarFeriadoDataUseCase,
            IDeletarFeriadoDataUseCase deletarFeriadoDataUseCase,
            ICriarFeriadoEstadoUseCase criarFeriadoEstadoUseCase,
            IDeletarFeriadoEstadoUseCase deletarFeriadoEstadoUseCase)
        {
            _atualizarFeriadoUseCase = atualizarFeriadoUseCase;
            _criarFeriadoUseCase = criarFeriadoUseCase;
            _listarFeriadoUseCase = listarFeriadoUseCase;
            _obterFeriadoUseCase = obterFeriadoUseCase;
            _criarFeriadoDataUseCase = criarFeriadoDataUseCase;
            _deletarFeriadoDataUseCase = deletarFeriadoDataUseCase;
            _criarFeriadoEstadoUseCase = criarFeriadoEstadoUseCase;
            _deletarFeriadoEstadoUseCase = deletarFeriadoEstadoUseCase;
        }

        [HttpPut]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<int>> AtualizarFeriado(FeriadoInput input)
        {
            input.UsuarioIdMod = await ObterUsuarioId();
            int id = await _atualizarFeriadoUseCase.Execute(input);

            if (id < 1)
            {
                return BadRequest();
            }

            await _deletarFeriadoDataUseCase.Execute(id);
            await _criarFeriadoDataUseCase.Execute(input.Data!, id);

            await _deletarFeriadoEstadoUseCase.Execute(id);
            await _criarFeriadoEstadoUseCase.Execute(input.EstadoId!, id);

            return Ok(id);
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<int>> CriarFeriado(FeriadoInput input)
        {
            input.UsuarioId = await ObterUsuarioId();
            int id = await _criarFeriadoUseCase.Execute(input);

            if (id < 1)
            {
                return BadRequest();
            }

            await _criarFeriadoDataUseCase.Execute(input.Data!, id);
            await _criarFeriadoEstadoUseCase.Execute(input.EstadoId!, id);

            return Ok(id);
        }

        [HttpGet("listar")]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LogOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(LogOutput))]
        public async Task<ActionResult<IEnumerable<FeriadoOutput>>> ListarFeriado([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarFeriadoUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpGet]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeriadoOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(FeriadoOutput))]
        public async Task<ActionResult<FeriadoOutput>> ObterFeriado(int id)
        {
            var item = await _obterFeriadoUseCase.Execute(id);

            if (item is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(item);
        }
    }
}