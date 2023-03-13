using AutoMapper;
using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auths.Registrar
{
    public sealed class RegistrarUseCase : IRegistrarUseCase
    {
        private readonly IMapper _map;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public RegistrarUseCase(
            IMapper map,
            IJwtTokenGenerator jwtTokenGenerator,
            ICriarUsuarioUseCase criarUsuarioUseCase,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _map = map;
            _jwtTokenGenerator = jwtTokenGenerator;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<(UsuarioOutput?, string)> Execute(RegistrarInput input)
        {
            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            //var verificarUsuario = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Email, input?.NomeUsuarioSistema);

            //if (verificarUsuario is not null)
            //{
            //    return (new UsuarioInput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioExistente));
            //}

            // #2.1 - Verificar requisitos gerais;
            if (input?.NomeCompleto?.Length < 3 || input?.NomeUsuarioSistema?.Length < 3)
            {
                return (new UsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.RequisitosNome));
            }

            // #2.2 - Verificar e-mail;
            if (!ValidarEmail(input?.Email!))
            {
                return (new UsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.EmailInvalido));
            }

            // #2.3 - Verificar requisitos de senha;
            var validarSenha = ValidarSenha(input?.Senha!, input?.NomeCompleto!, input?.NomeUsuarioSistema!, input?.Email!);
            if (!validarSenha.Item1)
            {
                return (new UsuarioOutput(), validarSenha.Item2);
            }

            // #3.1 - Gerar código de verificação para usar no processo de criação e no envio de e-mail;
            string codigoVerificacao = GerarStringAleatoria(6, true);

            // #3.2 - Criar usuário;
            input!.Senha = Criptografar(input?.Senha!);
            input!.HistPerfisAtivos = input?.UsuariosRolesId?.Length > 0 ? string.Join(", ", input.UsuariosRolesId) : string.Empty;
            UsuarioOutput output = await _criarUsuarioUseCase.Execute(_map.Map<UsuarioInput>(input));

            // #4 - Automaticamente atualizar o valor da Foto com um valor padrão após criar o novo usuário e adicionar ao ovjeto novoUsuario;
            //string nomeNovaFoto = $"{usuarioId}{GerarStringAleatoria(5, true)}.webp";
            //await _usuarioRepository.AtualizarFoto(usuarioId, nomeNovaFoto);
            //novoUsuario.Foto = nomeNovaFoto;

            // #5 - Criar token JWT;
            output.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: input?.NomeCompleto!, email: input?.Email!, listaClaims: null);

            // #6 - Gerar refresh token;
            output = await GerarRefreshToken(output!, output.UsuarioId);

            // #7 - Enviar e-mail de verificação de conta;
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

            return (output, string.Empty);
        }

        private async Task<UsuarioOutput> GerarRefreshToken(UsuarioOutput output, int usuarioId)
        {
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            output.RefreshToken = refreshToken;

            Domain.Entities.RefreshToken novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _criarRefreshTokenUseCase.Execute(novoRefreshToken);

            return output;
        }
    }
}