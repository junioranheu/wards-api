using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.RefreshToken;
using Wards.Application.UsesCases.Auths.Registrar;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : BaseController<Controller>
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<UsuarioInput>> Logar(UsuarioInput input)
        {
            (UsuarioInput?, string) authResultado = await _logarUseCase.Execute(input);

            if (!string.IsNullOrEmpty(authResultado.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, authResultado.Item2);

            return Ok(authResultado.Item1);
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<Usuario>> Registrar(UsuarioInput input)
        {
            (UsuarioInput?, string) authResultado = await _registrarUseCase.Execute(input);

            if (!string.IsNullOrEmpty(authResultado.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, authResultado.Item2);

            return Ok(authResultado.Item1);
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<Usuario>> RefreshToken(UsuarioInput input)
        {
            (UsuarioInput?, string) authResultado = await _refreshTokenUseCase.Execute(input.Token!, input.RefreshToken!, ObterUsuarioEmail());

            if (!string.IsNullOrEmpty(authResultado.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, authResultado.Item2);

            return Ok(authResultado.Item1);
        }
    }
}
