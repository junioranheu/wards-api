using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
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
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public RegistrarUseCase(
            IJwtTokenGenerator jwtTokenGenerator,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<(UsuarioInput?, string)> Execute(UsuarioInput input)
        {
            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            //var verificarUsuario = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Usuarios!.Email, input?.Usuarios!.NomeUsuarioSistema);

            //if (verificarUsuario is not null)
            //{
            //    return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioExistente));
            //}

            // #2.1 - Verificar requisitos gerais;
            if (input?.Usuarios!.NomeCompleto?.Length < 3 || input?.Usuarios!.NomeUsuarioSistema?.Length < 3)
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.RequisitosNome));
            }

            // #2.2 - Verificar e-mail;
            if (!ValidarEmail(input?.Usuarios!.Email!))
            {
                return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.EmailInvalido));
            }

            // #2.3 - Verificar requisitos de senha;
            var validarSenha = ValidarSenha(input?.Usuarios!.Senha!, input?.Usuarios!.NomeCompleto!, input?.Usuarios!.NomeUsuarioSistema!, input?.Usuarios!.Email!);
            if (!validarSenha.Item1)
            {
                return (new UsuarioInput(), validarSenha.Item2);
            }

            // #3.1 - Gerar código de verificação para usar no processo de criação e no envio de e-mail;
            string codigoVerificacao = GerarStringAleatoria(6, true);

            // #3.2 - Criar usuário;
            Usuario novoUsuario = new()
            {
                NomeCompleto = input?.Usuarios!.NomeCompleto,
                Email = input?.Usuarios!.Email,
                NomeUsuarioSistema = input?.Usuarios!.NomeUsuarioSistema,
                Senha = Criptografar(input?.Usuarios!.Senha!),
                Chamado = input?.Usuarios!.Chamado,
                HistPerfisAtivos = input?.Usuarios!.HistPerfisAtivos?.Length > 0 ? string.Join(", ", input.Usuarios!.HistPerfisAtivos) : string.Empty
            };

            int usuarioId = await _criarUsuarioUseCase.Execute(novoUsuario);

            // #4 - Automaticamente atualizar o valor da Foto com um valor padrão após criar o novo usuário e adicionar ao ovjeto novoUsuario;
            //string nomeNovaFoto = $"{usuarioId}{GerarStringAleatoria(5, true)}.webp";
            //await _usuarioRepository.AtualizarFoto(usuarioId, nomeNovaFoto);
            //novoUsuario.Foto = nomeNovaFoto;

            // #5 - Adicionar ao objeto novoUsuario o id do novo usuário;
            input!.Usuarios!.UsuarioId = usuarioId;

            // #6 - Criar token JWT;
            input.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: input?.Usuarios!.NomeCompleto!, email: input?.Usuarios!.Email!, listaClaims: null);

            // #7 - Gerar refresh token;
            input = await GerarRefreshToken(input!, usuarioId);

            // #8 - Enviar e-mail de verificação de conta;
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