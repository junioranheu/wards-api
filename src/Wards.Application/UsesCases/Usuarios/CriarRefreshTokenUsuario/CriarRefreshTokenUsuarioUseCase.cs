using System.Security.Claims;
using Wards.Application.UsesCases.Auths.Shared.Output;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.CriarRefreshTokenUsuario
{
    public sealed class CriarRefreshTokenUsuarioUseCase : ICriarRefreshTokenUsuarioUseCase
    {
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterRefreshTokenUseCase _obterRefreshTokenUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public CriarRefreshTokenUsuarioUseCase(
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

        public async Task<CriarRefreshTokenUsuarioOutput?> Execute(string token, string refreshToken, string email)
        {
            UsuarioOutput? usuario = await _obterUsuarioUseCase.Execute(email: email);

            if (usuario is null)
            {
                return (new CriarRefreshTokenUsuarioOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado) } });
            }

            int usuarioId = usuario.UsuarioId;
            var refreshTokenSalvoAnteriormente = await _obterRefreshTokenUseCase.Execute(usuarioId);

            if (refreshTokenSalvoAnteriormente != refreshToken)
            {
                return (new CriarRefreshTokenUsuarioOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.RefreshTokenInvalido) } });
            }

            ClaimsPrincipal? principal = _jwtTokenGenerator.GetInfoTokenExpirado(token);
            CriarRefreshTokenUsuarioOutput? output = await GerarRefreshToken(principal, usuarioId);

            return output;
        }

        private async Task<CriarRefreshTokenUsuarioOutput> GerarRefreshToken(ClaimsPrincipal? principal, int usuarioId)
        {
            var novoToken = _jwtTokenGenerator.GerarToken(nomeCompleto: string.Empty, email: string.Empty, listaClaims: principal?.Claims);
            var novoRefreshToken = _jwtTokenGenerator.GerarRefreshToken();

            // Criar novo registro com o novo refresh token gerado;
            RefreshTokenInput novoRefreshTokenInput = new()
            {
                RefToken = novoRefreshToken,
                UsuarioId = usuarioId
            };

            await _criarRefreshTokenUseCase.Execute(novoRefreshTokenInput);

            // Retornar o novo token e o novo refresh token;
            CriarRefreshTokenUsuarioOutput output = new()
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken
            };

            return output;
        }
    }
}