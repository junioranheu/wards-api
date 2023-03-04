using System.Security.Claims;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public sealed class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterRefreshTokenUseCase _obterRefreshTokenUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public RefreshTokenUseCase(
            IObterUsuarioUseCase obterUsuarioUseCase,
            IJwtTokenGenerator jwtTokenGenerator,
            IObterRefreshTokenUseCase obterRefreshTokenUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterRefreshTokenUseCase = obterRefreshTokenUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<(UsuarioInput?, string)> RefreshToken(string token, string refreshToken, string email)
        {
            Usuario? usuario = await _obterUsuarioUseCase.Obter(email: email);

            if (usuario is null)
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado));
            }

            int usuarioId = usuario.UsuarioId;
            var refreshTokenSalvoAnteriormente = await _obterRefreshTokenUseCase.ObterByUsuarioId(usuarioId);
            if (refreshTokenSalvoAnteriormente != refreshToken)
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.RefreshTokenInvalido));
            }

            ClaimsPrincipal? principal = _jwtTokenGenerator.GetInfoTokenExpirado(token);
            UsuarioInput output = await GerarRefreshToken(principal, usuarioId);

            return (output, string.Empty);
        }

        private async Task<UsuarioInput> GerarRefreshToken(ClaimsPrincipal? principal, int usuarioId)
        {
            var novoToken = _jwtTokenGenerator.GerarToken(nomeCompleto: string.Empty, email: string.Empty, listaClaims: principal?.Claims);
            var novoRefreshToken = _jwtTokenGenerator.GerarRefreshToken();

            // Criar novo registro com o novo refresh token gerado;
            Domain.Entities.RefreshToken novoRefreshTokenInput = new()
            {
                RefToken = novoRefreshToken,
                UsuarioId = usuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Criar(novoRefreshTokenInput);

            // Retornar o novo token e o novo refresh token;
            UsuarioInput input = new()
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken
            };

            return input;
        }
    }
}
