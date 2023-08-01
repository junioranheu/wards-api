using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.Hashtags.ListarHashtag;
using Wards.Domain.Consts;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HashtagsController : BaseController<HashtagsController>
    {
        private readonly IListarHashtagUseCase _listarUseCase;

        public HashtagsController(IListarHashtagUseCase listarUseCase)
        {
            _listarUseCase = listarUseCase;
        }

        [HttpGet("listar")]
        [ResponseCache(Duration = TemposConst.DezMinutosEmSegundos)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(List<string>))]
        public async Task<ActionResult<List<string>>> Listar()
        {
            var lista = await _listarUseCase.Execute();

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }
    }
}