using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public sealed class LogarUseCase : ILogarUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public LogarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<(UsuarioInput?, string)> Execute(UsuarioInput input)
        {
            // #1 - Buscar usuário e sua senha (para não expor no output);
            (UsuarioOutput?, string) resp = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Email, input?.NomeUsuarioSistema);
            UsuarioOutput? usuario = resp.Item1;
            string senha = resp.Item2;

            if (usuario is null)
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioNaoEncontrado));
            }

            // #2 - Verificar se a senha está correta;
            if (senha != Criptografar(input!.Senha ?? string.Empty))
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioSenhaIncorretos));
            }

            // #3 - Verificar se o usuário está ativo;
            if (!usuario.IsAtivo)
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.ContaDesativada));
            }

            // #4 - Criar token JWT;
            input!.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: usuario.NomeCompleto!, email: usuario.Email!, listaClaims: null);

            // #5 - Gerar refresh token;
            input = await GerarRefreshToken(input, usuario.UsuarioId);

            return (input, string.Empty);
        }

        private async Task<UsuarioInput> GerarRefreshToken(UsuarioInput input, int usuarioId)
        {
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            input.RefreshToken = refreshToken;

            Domain.Entities.RefreshToken novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Execute(novoRefreshToken);

            return input;
        }
    }
}