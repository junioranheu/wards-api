using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.RefreshToken;
using Wards.Application.UsesCases.Auths.Registrar;
using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Auths.Shared.Output;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Domain.Enums;

namespace Wards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController<AuthController>
    {
        private readonly ILogarUseCase _logarUseCase;
        private readonly IRegistrarUseCase _registrarUseCase;
        private readonly IRefreshTokenUseCase _refreshTokenUseCase;
        private readonly ICriarUsuarioRoleUseCase _criarUsuarioRoleUseCase;

        public AuthController(
            ILogarUseCase logarUseCase,
            IRegistrarUseCase registrarUseCase,
            IRefreshTokenUseCase refreshTokenUseCase,
            ICriarUsuarioRoleUseCase criarUsuarioRoleUseCase)
        {
            _logarUseCase = logarUseCase;
            _registrarUseCase = registrarUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
            _criarUsuarioRoleUseCase = criarUsuarioRoleUseCase;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> Logar(LogarInput input)
        {
            var resp = await _logarUseCase.Execute(input);

            if (resp.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return Ok(resp);
        }

        [HttpPost("registrar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> Registrar(RegistrarInput input)
        {
            var resp = await _registrarUseCase.Execute(input);

            if (resp!.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            await _criarUsuarioRoleUseCase.Execute(input.UsuariosRolesId!, resp.UsuarioId);

            return Ok(resp);
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthsRefreshTokenOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<AuthsRefreshTokenOutput>> RefreshToken(AuthsRefreshTokenInput input)
        {
            var resp = await _refreshTokenUseCase.Execute(input.Token!, input.RefreshToken!, ObterUsuarioEmail());

            if (resp!.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return Ok(resp);
        }

        [HttpPut("verificarConta/{codigoVerificacao}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(bool))]
        public async Task<ActionResult<bool>> VerificarConta(string codigoVerificacao)
        {
            var resp = await _usuarios.VerificarConta(codigoVerificacao);
            return resp;
        }
    }
}