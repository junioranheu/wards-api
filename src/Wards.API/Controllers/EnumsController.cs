using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Domain.Consts;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : Controller
    {
        [HttpGet("listarUsuarioRole")]
        [ResponseCache(Duration = TemposConst.UmaHoraEmSegundos)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<dynamic>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<List<dynamic>> ListarUsuarioRole()
        {
            var lista = ListarEnum<UsuarioRoleEnum>();

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpGet("listarCodigoErro")]
        [ResponseCache(Duration = TemposConst.UmaHoraEmSegundos)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<dynamic>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<List<dynamic>> ListarCodigoErro()
        {
            var lista = ListarEnum<CodigoErroEnum>();

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }
    }
}