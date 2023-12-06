using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.Auxiliares.ListarEstado;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Consts;
using Wards.Domain.Enums;
using Wards.Infrastructure.Factory.ConnectionFactory;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuxiliaresController : Controller
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IListarEstadoUseCase _listarEstadoUseCase;

        public AuxiliaresController(
            IConnectionFactory connectionFactory,
            IListarEstadoUseCase listarEstadoUseCase)
        {
            _connectionFactory = connectionFactory;
            _listarEstadoUseCase = listarEstadoUseCase;
        }

        [HttpGet("obterStatusBanco")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((bool, string)))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<(bool, string)>> ObterStatusBanco(CancellationToken cancellationToken = default)
        {
            bool isOk = true;
            string desc = string.Empty;

            try
            {
                var connection = _connectionFactory.ObterMySqlConnection();

                await connection.OpenAsync(cancellationToken);
                return Ok(new { isOk, desc });
            }
            catch (Exception ex)
            {
                isOk = false;
                return Ok(new { isOk, ex.Message});
            }
        }

        [HttpGet("listarEstado")]
        [ResponseCache(Duration = TemposConst.UmMesEmSegundos)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstadoOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(EstadoOutput))]
        public async Task<ActionResult<IEnumerable<EstadoOutput>>> ListarEstado([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarEstadoUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }
    }
}