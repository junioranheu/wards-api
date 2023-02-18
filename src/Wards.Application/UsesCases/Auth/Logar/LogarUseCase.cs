using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Models;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Autenticar.Logar
{
    public sealed class LogarUseCase : ILogarUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioUseCase obterUsuarioUseCase,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<UsuarioDTO> Login(UsuarioDTO dto)
        {
            // #1 - Verificar se o usuário existe;
            var usuario = await _usuarioRepository.GetByEmailOuUsuarioSistema(dto?.Email, dto?.NomeUsuarioSistema);

            if (usuario is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioNaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioNaoEncontrado) };
                return erro;
            }

            // #2 - Verificar se a senha está correta;
            if (usuario.Senha != Criptografar(dto?.Senha))
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

            RefreshTokenDTO novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuario.UsuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _refreshTokenRepository.Adicionar(novoRefreshToken);

            // #6 - Converter de UsuarioSenhaDTO para UsuarioDTO;
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(usuario);

            return usuarioDTO;
        }
    }
}
