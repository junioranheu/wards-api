using Microsoft.AspNetCore.Mvc;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : Controller
    {
        private readonly ILogarUseCase _logarService;

        public AutenticarController(ILogarUseCase logarService)
        {
            _logarService = logarService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Logar(Usuario input)
        {
            var authResultado = await _logarService.Logar(input);
            return Ok(authResultado);
        }

        //[HttpPost("registrar")]
        //public async Task<ActionResult<Usuario>> Registrar(Usuario input)
        //{
        //    var authResultado = await _autenticarService.Registrar(input);
        //    return Ok(authResultado);
        //}

        //[HttpPost("refreshToken")]
        //public async Task<ActionResult<Usuario>> RefreshToken(UsuarioDTO input)
        //{
        //    var authResultado = await _autenticarService.RefreshToken(input.Token, input.RefreshToken);
        //    return Ok(authResultado);
        //}
    }
}
