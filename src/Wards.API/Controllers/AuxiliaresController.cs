using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Auxiliares.ListarEstado;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;

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
        [AuthorizeFilter]
        public async Task<ActionResult<IEnumerable<EstadoOutput>>> ListarEstado()
        {
            var lista = await _listarEstadoUseCase.ExecuteAsync();

            if (lista is null)
                return NotFound();

            return Ok(lista);
        }
    }
}