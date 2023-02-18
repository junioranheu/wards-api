using Microsoft.AspNetCore.Mvc;
using Wards.Application.UsesCases.Usuarios.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : Controller
    {
        private readonly IAutenticarService _autenticarService;

        public AutenticarController(IAutenticarService autenticarService)
        {
            _autenticarService = autenticarService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Login(Usuario input)
        {
            var authResultado = await _autenticarService.Login(input);
            return Ok(authResultado);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(Usuario input)
        {
            var authResultado = await _autenticarService.Registrar(input);
            return Ok(authResultado);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UsuarioDTO>> RefreshToken(UsuarioDTO input)
        {
            var authResultado = await _autenticarService.RefreshToken(input.Token, input.RefreshToken);
            return Ok(authResultado);
        }
    }
}
