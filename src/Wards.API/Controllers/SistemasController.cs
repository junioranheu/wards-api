using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Application.Services.Sistemas.ResetarBancoDados;
using static Wards.Utils.Common;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SistemasController : Controller
    {
        private readonly IResetarBancoDadosService _resetarBancoDadosService;

        public SistemasController(IResetarBancoDadosService resetarBancoDadosService)
        {
            _resetarBancoDadosService = resetarBancoDadosService;
        }

        /// <summary>
        /// Método provisório para resetar, recriar e seedar a estrutura do banco de dados completa;
        /// A criação do método foi necessária para criação da base em QA;
        /// Existe uma verificação simplória (parâmetro "minuto") porventura;
        /// !!! Em produção, esse método deve ser terminantemente removido ou, pelo menos, ocultado;
        /// </summary>
#if DEBUG
        [ApiExplorerSettings(IgnoreApi = false)]
#else
        [ApiExplorerSettings(IgnoreApi = true)]
#endif  
        [HttpGet("resetarBancoDados")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(bool))]
        public async Task<ActionResult<bool>> ResetarBancoDados(int minuto)
        {
            if (HorarioBrasilia().Minute != minuto)
                return StatusCode(StatusCodes.Status403Forbidden, false);

            bool resp = await _resetarBancoDadosService.ExecuteAsync();

            if (!resp)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return Ok(resp);
        }
    }
}