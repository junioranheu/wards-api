using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public sealed class LogarUseCase : ILogarUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly ICriarRefreshTokenUsecase _criarRefreshTokenUseCase;

        public LogarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarRefreshTokenUsecase criarRefreshTokenUseCase)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<UsuarioDTO> Logar(Usuario input)
        {
            Usuario usuario = await _obterUsuarioUseCase.ObterByEmailOuUsuarioSistema(input?.Email, input?.NomeUsuarioSistema);

            if (usuario is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioNaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioNaoEncontrado) };
                return erro;
            }

            // #2 - Verificar se a senha está correta;
            if (usuario.Senha != Criptografar(input?.Senha ?? string.Empty))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioSenhaIncorretos, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioSenhaIncorretos) };
                return erro;
            }

            // #3 - Verificar se o usuário está ativo;
            if (!usuario.IsAtivo)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaDesativada, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaDesativada) };
                return erro;
            }

            // #4 - Criar token JWT;
            var token = _jwtTokenGenerator.GerarToken(usuario, null);
            usuario.Token = token;

            // #5 - Gerar refresh token;
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            usuario.RefreshToken = refreshToken;

            RefreshToken novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuario.UsuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Criar(novoRefreshToken);

            return usuario;
        }

        private async Task<Usuario> ObterByEmailOuUsuarioSistema(Usuario input)
        {

        }
    }
}
