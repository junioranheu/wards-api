using Microsoft.AspNetCore.Mvc;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.Registrar;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : Controller
    {
        private readonly ILogarUseCase _logarService;
        private readonly IRegistrarUseCase _registrarService;

        public AutenticarController(ILogarUseCase logarService, IRegistrarUseCase registrarService)
        {
            _logarService = logarService;
            _registrarService = registrarService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Logar(Usuario input)
        {
            var authResultado = await _logarService.Logar(input);
            return Ok(authResultado);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(Usuario input)
        {
            var authResultado = await _registrarService.Registrar(input);
            return Ok(authResultado);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<Usuario>> RefreshToken(UsuarioDTO input)
        {
            var authResultado = await _autenticarService.RefreshToken(input.Token, input.RefreshToken);
            return Ok(authResultado);
        }
    }
}
