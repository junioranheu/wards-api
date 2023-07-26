using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WardsHashtagsController : BaseController<WardsHashtagsController>
    {
        private readonly IListarHashtagQtdUseCase _listarUseCase;

        public WardsHashtagsController(IListarHashtagQtdUseCase listarUseCase)
        {
            _listarUseCase = listarUseCase;
        }

        [HttpGet("listarHashtagQtd")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HashtagQtdOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(HashtagQtdOutput))]
        public async Task<ActionResult<IEnumerable<HashtagQtdOutput>>> ListarHashtagQtd()
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