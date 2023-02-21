using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.RefreshToken;
using Wards.Application.UsesCases.Auths.Registrar;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : Controller
    {
        private readonly ILogarUseCase _logarUseCase;
        private readonly IRegistrarUseCase _registrarUseCase;
        private readonly IRefreshTokenUseCase _refreshTokenUseCase;

        public AutenticarController(
            ILogarUseCase logarUseCase,
            IRegistrarUseCase registrarUseCase,
            IRefreshTokenUseCase refreshTokenUseCase)
        {
            _logarUseCase = logarUseCase;
            _registrarUseCase = registrarUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Logar(Usuario input)
        {
            var authResultado = await _logarUseCase.Logar(input);
            return Ok(authResultado);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(Usuario input)
        {
            var authResultado = await _registrarUseCase.Registrar(input);
            return Ok(authResultado);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<Usuario>> RefreshToken(UsuarioDTO input)
        {
            string email = HttpContext.User.FindFirst(ClaimTypes.Email).Value ?? string.Empty;
            var authResultado = await _refreshTokenUseCase.RefreshToken(input.Token ?? string.Empty, input.RefreshToken ?? string.Empty, email);
            return Ok(authResultado);
        }
    }
}
