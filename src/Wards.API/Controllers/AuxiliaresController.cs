using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.Auxiliares.ListarEstado;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Shared.Models;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuxiliaresController : Controller
    {
        private readonly IListarEstadoUseCase _listarEstadoUseCase;

        public AuxiliaresController(IListarEstadoUseCase listarEstadoUseCase)
        {
            _listarEstadoUseCase = listarEstadoUseCase;
        }

        [HttpGet("listarEstado")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstadoOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(EstadoOutput))]
        public async Task<ActionResult<IEnumerable<EstadoOutput>>> ListarEstado([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarEstadoUseCase.Execute(input);

            if (!lista.Any())
            {
                return StatusCode(StatusCodes.Status404NotFound, new EstadoOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });
            }

            return Ok(lista);
        }
    }
}