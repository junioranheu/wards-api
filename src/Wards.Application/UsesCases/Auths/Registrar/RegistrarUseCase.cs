using AutoMapper;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.Registrar
{
    public sealed class RegistrarUseCase : IRegistrarUseCase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;
        private readonly IMapper _map;

        public RegistrarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IObterUsuarioUseCase obterUsuarioUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase,
            IMapper map)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _obterUsuarioUseCase = obterUsuarioUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
            _map = map;
        }

        public async Task<UsuarioDTO> Registrar(Usuario input)
        {
            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            var verificarUsuario = await _obterUsuarioUseCase.ObterByEmailOuUsuarioSistema(input?.Email, input?.NomeUsuarioSistema);

            if (verificarUsuario is not null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.UsuarioExistente, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.UsuarioExistente) };
                return erro;
            }

            // #2.1 - Verificar requisitos gerais;
            if (input?.NomeCompleto?.Length < 3 || input?.NomeUsuarioSistema?.Length < 3)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.RequisitosNome, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.RequisitosNome) };
                return erro;
            }

            // #2.2 - Verificar e-mail;
            if (!ValidarEmail(input?.Email ?? string.Empty))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.EmailInvalido, MensagemErro = GetDescricaoEnum(CodigosErrosEnum.EmailInvalido) };
                return erro;
            }

            // #2.3 - Verificar requisitos de senha;
            var validarSenha = ValidarSenha(input?.Senha ?? string.Empty, input?.NomeCompleto ?? string.Empty, input?.NomeUsuarioSistema ?? string.Empty, input?.Email ?? string.Empty);
            if (!validarSenha.Item1)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigosErrosEnum.RequisitosSenhaNaoCumprido, MensagemErro = validarSenha.Item2 };
                return erro;
            }

            // #3.1 - Gerar código de verificação para usar no processo de criação e no envio de e-mail;
            string codigoVerificacao = GerarStringAleatoria(6, true);

            // #3.2 - Criar usuário;
            Usuario novoUsuario = new()
            {
                NomeCompleto = input?.NomeCompleto,
                Email = input?.Email,
                NomeUsuarioSistema = input?.NomeUsuarioSistema,
                Senha = Criptografar(input?.Senha ?? string.Empty),
                Data = HorarioBrasilia(),
                IsAtivo = true
            };

            UsuarioDTO usuarioAdicionado = await _criarUsuarioUseCase.Criar(novoUsuario);
            int usuarioId = usuarioAdicionado.UsuarioId;

            // #4 - Automaticamente atualizar o valor da Foto com um valor padrão após criar o novo usuário e adicionar ao ovjeto novoUsuario;
            //string nomeNovaFoto = $"{usuarioId}{GerarStringAleatoria(5, true)}.webp";
            //await _usuarioRepository.AtualizarFoto(usuarioId, nomeNovaFoto);
            //novoUsuario.Foto = nomeNovaFoto;

            // #5 - Converter de UsuarioSenhaDTO para UsuarioDTO;
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(novoUsuario);

            // #6 - Adicionar ao objeto novoUsuario o id do novo usuário;
            usuarioDTO.UsuarioId = usuarioId;

            // #7 - Criar token JWT;
            var token = _jwtTokenGenerator.GerarToken(usuarioDTO, null);
            usuarioDTO.Token = token;

            // #8 - Gerar refresh token;
            usuarioDTO = await GerarRefreshToken(usuarioDTO, usuarioId);

            // #9 - Enviar e-mail de verificação de conta;
            //try
            //{
            //    if (!String.IsNullOrEmpty(usuarioDTO?.Email) && !String.IsNullOrEmpty(usuarioDTO?.NomeCompleto) && !String.IsNullOrEmpty(codigoVerificacao))
            //    {
            //        usuarioDTO.IsEmailVerificacaoContaEnviado = await EnviarEmailVerificacaoConta(usuarioDTO?.Email, usuarioDTO?.NomeCompleto, codigoVerificacao);
            //    }
            //}
            //catch (Exception)
            //{
            //    usuarioDTO.IsEmailVerificacaoContaEnviado = false;
            //}

            return usuarioDTO;
        }

        private async Task<UsuarioDTO> GerarRefreshToken(UsuarioDTO dto, int usuarioId)
        {
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            dto.RefreshToken = refreshToken;

            Domain.Entities.RefreshToken novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Criar(novoRefreshToken);

            return dto;
        }
    }
}