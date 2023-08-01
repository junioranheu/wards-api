using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.Hashtags.ListarHashtag;
using Wards.Application.UseCases.Hashtags.Shared.Output;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HashtagOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(IEnumerable<HashtagOutput>))]
        public async Task<ActionResult<IEnumerable<HashtagOutput>>> Listar()
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