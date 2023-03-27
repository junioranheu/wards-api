using AutoMapper;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : BaseUsuario, ICriarUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICriarUsuarioCommand _criarCommand;
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public CriarUsuarioUseCase(
            IMapper map,
            IJwtTokenGenerator jwtTokenGenerator,
            ICriarUsuarioCommand criarCommand,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _map = map;
            _jwtTokenGenerator = jwtTokenGenerator;
            _criarCommand = criarCommand;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<AutenticarUsuarioOutput?> Execute(CriarUsuarioInput input)
        {
            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            //var verificarUsuario = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Email, input?.NomeUsuarioSistema);

            //if (verificarUsuario is not null)
            //{
            //    return (new AutenticarUsuarioOutput(), GetDescricaoEnum(CodigosErrosEnum.UsuarioExistente));
            //}

            // #2.1 - Verificar requisitos gerais;
            if (input?.NomeCompleto?.Length < 3 || input?.NomeUsuarioSistema?.Length < 3)
                return (new AutenticarUsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.RequisitosNome) } });

            // #2.2 - Verificar e-mail;
            if (!ValidarEmail(input?.Email!))
                return (new AutenticarUsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.EmailInvalido) } });

            // #2.3 - Verificar requisitos de senha;
            var validarSenha = ValidarSenha(input?.Senha!, input?.NomeCompleto!, input?.NomeUsuarioSistema!, input?.Email!);
            if (!validarSenha.Item1)
                return (new AutenticarUsuarioOutput() { Messages = new string[] { validarSenha.Item2 } });

            // #3.1 - Gerar código de verificação para usar no processo de criação e no envio de e-mail;
            string codigoVerificacao = GerarStringAleatoria(6, true);

            // #3.2 - Criar usuário;
            input!.CodigoVerificacao = codigoVerificacao;
            input!.ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24);
            input!.Senha = Criptografar(input?.Senha!);
            input!.HistPerfisAtivos = input?.UsuariosRolesId?.Length > 0 ? string.Join(", ", input.UsuariosRolesId) : string.Empty;
            AutenticarUsuarioOutput output = _map.Map<AutenticarUsuarioOutput>(await _criarCommand.Execute(_map.Map<Usuario>(input)));

            // #4 - Automaticamente atualizar o valor da Foto com um valor padrão após criar o novo usuário e adicionar ao ovjeto novoUsuario;
            //string nomeNovaFoto = $"{usuarioId}{GerarStringAleatoria(5, true)}.webp";
            //await _usuarioRepository.AtualizarFoto(usuarioId, nomeNovaFoto);
            //novoUsuario.Foto = nomeNovaFoto;

            // #5 - Criar token JWT;
            output.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: input?.NomeCompleto!, email: input?.Email!, listaClaims: null);

            // #6 - Gerar refresh token;
            output = await GerarRefreshToken(output!, output.UsuarioId);

            // #7 - Enviar e-mail de verificação de conta;
            if (!string.IsNullOrEmpty(output?.Email) && !string.IsNullOrEmpty(output?.NomeCompleto) && !string.IsNullOrEmpty(codigoVerificacao))
            {
                await EnviarEmailVerificacaoConta(output.Email, output.NomeCompleto, codigoVerificacao);
            }

            return output;
        }

        private async Task<AutenticarUsuarioOutput> GerarRefreshToken(AutenticarUsuarioOutput output, int usuarioId)
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