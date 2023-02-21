using AutoMapper;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.DTOs;
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
        private readonly IMapper _map;

        public LogarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase,
            IMapper map)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
            _map = map;
        }

        public async Task<UsuarioDTO> Logar(Usuario input)
        {
            // #1 - Buscar usuário;
            Usuario usuario = await _obterUsuarioUseCase.ObterByEmailOuUsuarioSistema(input?.Email, input?.NomeUsuarioSistema);

            if (usuario is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.UsuarioNaoEncontrado, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.UsuarioNaoEncontrado) };
                return erro;
            }

            // #2 - Verificar se a senha está correta;
            if (usuario.Senha != Criptografar(input?.Senha ?? string.Empty))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.UsuarioSenhaIncorretos, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.UsuarioSenhaIncorretos) };
                return erro;
            }

            // #3 - Verificar se o usuário está ativo;
            if (!usuario.IsAtivo)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.ContaDesativada, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.ContaDesativada) };
                return erro;
            }

            // #4 - AutoMapper;
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(usuario);

            // #5 - Criar token JWT;
            usuarioDTO.Token = _jwtTokenGenerator.GerarToken(usuarioDTO, null);

            // #6 - Gerar refresh token;
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            usuarioDTO.RefreshToken = refreshToken;

            Domain.Entities.RefreshToken novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuario.UsuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Criar(novoRefreshToken);

            return usuarioDTO;
        }
    }
}
