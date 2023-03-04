using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public sealed class LogarUseCase : ILogarUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public LogarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<(UsuarioInput?, string)> Logar(UsuarioInput input)
        {
            string msgErro = string.Empty;

            // #1 - Buscar usuário;
            Usuario usuario = await _obterUsuarioUseCase.ObterByEmailOuUsuarioSistema(input?.Usuarios!.Email, input?.Usuarios!.NomeUsuarioSistema);

            if (usuario is null)
            {
                msgErro = GetDescricaoEnum(CodigosErrosEnum.UsuarioNaoEncontrado);
                return (new UsuarioInput(), msgErro);
            }

            // #2 - Verificar se a senha está correta;
            if (usuario.Senha != Criptografar(input?.Usuarios!.Senha ?? string.Empty))
            {
                msgErro = GetDescricaoEnum(CodigosErrosEnum.UsuarioSenhaIncorretos);
                return (new UsuarioInput(), msgErro);
            }

            // #3 - Verificar se o usuário está ativo;
            if (!usuario.IsAtivo)
            {
                msgErro = GetDescricaoEnum(CodigosErrosEnum.ContaDesativada);
                return (new UsuarioInput(), msgErro);
            }

            // #4 - Criar token JWT;
            input!.Token = _jwtTokenGenerator.GerarToken(input.Usuarios!.NomeCompleto!, input.Usuarios!.Email!, null);

            // #5 - Gerar refresh token;
            input = await GerarRefreshToken(input, usuario.UsuarioId);

            return (input, msgErro);
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

            await _criarRefreshTokenUseCase.Criar(novoRefreshToken);

            return input;
        }
    }
}
