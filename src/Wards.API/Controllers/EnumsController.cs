using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using Wards.Application.UseCases.Shared.Models.Output;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EnumOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<List<EnumOutput>> ListarUsuarioRole()
        {
            List<EnumOutput> lista = ListarEnum<UsuarioRoleEnum>();

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpGet("listarCodigoErro")]
        [ResponseCache(Duration = TemposConst.UmaHoraEmSegundos)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EnumOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<List<EnumOutput>> ListarCodigoErro()
        {
            List<EnumOutput> lista = ListarEnum<CodigoErroEnum>();

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        #region metodos_auxiliares
        /// <summary>
        /// Recebe um <Enum> e lista todos os valores dele mapeados pela classe de resposta "EnumOutput";
        /// O método trata o Enum caso ele tenha/não tenha objetos com "[Description]";
        /// </summary>
        private static List<EnumOutput> ListarEnum<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).
                   Cast<TEnum>().
                   Select(x =>
                   {
                       FieldInfo? info = x.GetType().GetField(x.ToString());
                       string desc = info!.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute ? descriptionAttribute.Description : x.ToString();
                       return new EnumOutput { Id = (int)(object)x, Item = desc };
                   }).ToList();
        }
        #endregion
    }
}