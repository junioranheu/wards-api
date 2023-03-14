using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.RefreshToken;
using Wards.Application.UsesCases.Auths.RefreshToken.Models;
using Wards.Application.UsesCases.Auths.Registrar;
using Wards.Application.UsesCases.Auths.Shared.Input;
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
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<UsuarioOutput>> Logar(LogarInput input)
        {
            (UsuarioOutput?, string) resp = await _logarUseCase.Execute(input);

            if (!string.IsNullOrEmpty(resp.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, resp.Item2);

            return Ok(resp.Item1);
        }

        [HttpPost("registrar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<UsuarioOutput>> Registrar(RegistrarInput input)
        {
            (UsuarioOutput?, string) resp = await _registrarUseCase.Execute(input);

            if (!string.IsNullOrEmpty(resp.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, resp.Item2);

            await _criarUsuarioRoleUseCase.Execute(input.UsuariosRolesId!, (int)resp.Item1!.UsuarioId!);

            return Ok(resp.Item1);
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefreshTokenOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<RefreshTokenOutput>> RefreshToken(AuthsRefreshTokenInput input)
        {
            (RefreshTokenOutput?, string) resp = await _refreshTokenUseCase.Execute(input.Token!, input.RefreshToken!, ObterUsuarioEmail());

            if (!string.IsNullOrEmpty(resp.Item2))
                return StatusCode(StatusCodes.Status403Forbidden, resp.Item2);

            return Ok(resp.Item1);
        }
    }
}