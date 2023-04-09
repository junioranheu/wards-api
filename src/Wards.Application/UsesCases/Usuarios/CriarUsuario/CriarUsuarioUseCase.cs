using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _map;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICriarUsuarioCommand _criarCommand;
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public CriarUsuarioUseCase(
            IWebHostEnvironment webHostEnvironment,
            IMapper map,
            IJwtTokenGenerator jwtTokenGenerator,
            ICriarUsuarioCommand criarCommand,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _webHostEnvironment = webHostEnvironment;
            _map = map;
            _jwtTokenGenerator = jwtTokenGenerator;
            _criarCommand = criarCommand;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        /// <summary>
        /// A primeira verificação (#1) se o usuário existe não faz sentido em sistemas de gestão onde o ADM cria as contas;
        /// Porque, primeiro: o administrador cria as contas;
        /// Segundo: existe a lógica do isLatest e "replacement" dos dados;
        /// Portanto esse trecho do código está comentado;
        /// Mas... caso o sistema permita que o usuário final crie sua conta, esse trecho deve ser descomentado.
        /// </summary>
        public async Task<AutenticarUsuarioOutput?> Execute(CriarUsuarioInput input)
        {
            //var verificarUsuario = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Email, input?.NomeUsuarioSistema);

            //if (verificarUsuario is not null) {
            //  return (new AutenticarUsuarioOutput(), ObterDescricaoEnum(CodigosErrosEnum.UsuarioExistente));
            //}

            if (input?.NomeCompleto?.Length < 3 || input?.NomeUsuarioSistema?.Length < 3)
            {
                return (new AutenticarUsuarioOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.RequisitosNome) } });
            }

            if (!ValidarEmail(input?.Email!))
            {
                return (new AutenticarUsuarioOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.EmailInvalido) } });
            }

            var (isValido, mensagemErro) = ValidarSenha(input?.Senha!, input?.NomeCompleto!, input?.NomeUsuarioSistema!, input?.Email!);
            if (!isValido)
            {
                return (new AutenticarUsuarioOutput() { Messages = new string[] { mensagemErro } });
            }

            string codigoVerificacao = GerarStringAleatoria(6, true);
            AutenticarUsuarioOutput output = await CriarUsuario(input!, codigoVerificacao);

            if (output is null)
            {
                return output;
            }

            IFormFile? arquivo = ObterFotoAleatoria(_webHostEnvironment);
            output.Foto = await VerificarParametrosDepoisUparFoto(_webHostEnvironment, output.UsuarioId, arquivo);

            output.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: input?.NomeCompleto!, email: input?.Email!, listaClaims: null);
            output = await GerarRefreshToken(_jwtTokenGenerator, _criarRefreshTokenUseCase, output!, output.UsuarioId);

            if (!string.IsNullOrEmpty(output?.Email) && !string.IsNullOrEmpty(output?.NomeCompleto) && !string.IsNullOrEmpty(codigoVerificacao))
            {
                await EnviarEmailVerificacaoConta(output.Email, output.NomeCompleto, codigoVerificacao);
            }

            return output;
        }

        private async Task<AutenticarUsuarioOutput> CriarUsuario(CriarUsuarioInput input, string codigoVerificacao)
        {
            input!.CodigoVerificacao = codigoVerificacao;
            input!.ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24);
            input!.Senha = Criptografar(input?.Senha!);
            input!.HistPerfisAtivos = input?.UsuariosRolesId?.Length > 0 ? string.Join(", ", input.UsuariosRolesId) : string.Empty;

            return _map.Map<AutenticarUsuarioOutput>(await _criarCommand.Execute(_map.Map<Usuario>(input)));
        }
    }
}