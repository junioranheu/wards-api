using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.DTOs;
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

        public async Task<UsuarioDTO> RefreshToken(string token, string refreshToken, string email)
        {
            var principal = _jwtTokenGenerator.GetInfoTokenExpirado(token);
            UsuarioDTO usuarioDTO = await _obterUsuarioUseCase.ObterByEmailComCache(email);

            if (usuarioDTO is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.NaoEncontrado, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) };
                return erro;
            }

            int usuarioId = usuarioDTO.UsuarioId;
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
