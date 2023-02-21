using System.Security.Claims;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken;
using Wards.Domain.DTOs;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public sealed class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterRefreshTokenUseCase _obterRefreshTokenUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public RefreshTokenUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterRefreshTokenUseCase obterRefreshTokenUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterRefreshTokenUseCase = obterRefreshTokenUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<UsuarioDTO> RefreshToken(string token, string refreshToken)
        {
            var principal = _jwtTokenGenerator.GetInfoTokenExpirado(token);
            int usuarioId = Convert.ToInt32(principal?.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault());

            var refreshTokenSalvoAnteriormente = await _obterRefreshTokenUseCase.ObterByUsuarioId(usuarioId);
            if (refreshTokenSalvoAnteriormente != refreshToken)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.RefreshTokenInvalido, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.RefreshTokenInvalido) };
                return erro;
            }

            var novoToken = _jwtTokenGenerator.GerarToken(null, principal?.Claims);
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
            UsuarioDTO dto = new()
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken
            };

            return dto;
        }
    }
}
