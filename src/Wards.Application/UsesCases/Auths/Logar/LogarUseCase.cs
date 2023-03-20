using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
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

        public async Task<(UsuarioOutput?, string)> Execute(LogarInput input)
        {
            // #1 - Buscar usuário e sua senha (para não expor no output);
            (UsuarioOutput?, string) resp = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Login ?? string.Empty);
            UsuarioOutput? output = resp.Item1;
            string senhaCriptografada = resp.Item2;

            if (output is null)
            {
                return (new UsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioNaoEncontrado));
            }

            // #2 - Verificar se a senha está correta;
            if (!VerificarCriptografia(senha: input?.Senha ?? string.Empty, senhaCriptografada: senhaCriptografada)) {
                return (new UsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioSenhaIncorretos));
            }

            // #3 - Verificar se o usuário está ativo;
            if (!output.IsAtivo)
            {
                return (new UsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.ContaDesativada));
            }

            // #4 - Criar token JWT;
            output!.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: output.NomeCompleto!, email: output.Email!, listaClaims: null);

            // #5 - Gerar refresh token;
            output = await GerarRefreshToken(output, output.UsuarioId);

            return (output, string.Empty);
        }

        private async Task<UsuarioOutput> GerarRefreshToken(UsuarioOutput output, int usuarioId)
        {
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            output.RefreshToken = refreshToken;

            RefreshTokenInput novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuarioId
            };

            await _criarRefreshTokenUseCase.Execute(novoRefreshToken);

            return output;
        }
    }
}