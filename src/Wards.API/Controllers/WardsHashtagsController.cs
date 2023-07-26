using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Application.UseCases.WardsHashtags.ListarWardHashtag;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WardsHashtagsController : BaseController<WardsHashtagsController>
    {
        private readonly IListarWardHashtagUseCase _listarUseCase;

        public WardsHashtagsController(IListarWardHashtagUseCase listarUseCase)
        {
            _listarUseCase = listarUseCase;
        }

        [HttpGet("listar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WardHashtagOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WardOutput))]
        public async Task<ActionResult<IEnumerable<WardHashtagOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }
    }
}