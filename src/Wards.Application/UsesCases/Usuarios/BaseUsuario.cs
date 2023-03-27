using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using Wards.Utils.Entities;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios
{
    public class BaseUsuario
    {
        public BaseUsuario() { }

        internal static async Task<AutenticarUsuarioOutput> GerarRefreshToken(
            IJwtTokenGenerator _jwtTokenGenerator,
            ICriarRefreshTokenUseCase _criarRefreshTokenUseCase,
            AutenticarUsuarioOutput output,
            int usuarioId)
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

        internal static async Task<bool> EnviarEmailVerificacaoConta(string emailTo, string nomeUsuario, string codigoVerificacao)
        {
            try
            {
                const string assunto = "Verifique sua conta";
                string nomeArquivo = GetDescricaoEnum(EmailEnum.VerificarConta);

                List<EmailDadosReplace> listaDadosReplace = new()
                {
                    new EmailDadosReplace { Key = "Url", Value = $"{CaminhoFront()}/usuario/verificar-conta/{codigoVerificacao}"},
                    new EmailDadosReplace { Key = "NomeUsuario", Value = nomeUsuario }
                };

                bool resposta = await EnviarEmail(emailTo, assunto, nomeArquivo, listaDadosReplace);
                return resposta;
            }
            catch (Exception)
            {
                return false;
            }
        }


        internal static IFormFile GerarFotoAleatoria()
        {
            return null;
        }

        internal static async Task<bool> VerificarParametrosDepoisUparFoto(IWebHostEnvironment _webHostEnvironment, int usuarioId, IFormFile arquivo)
        {
            try
            {
                string nomeFoto = GerarNomeFoto(usuarioId);

                if (!string.IsNullOrEmpty(nomeFoto) && arquivo is not null)
                {
                    string caminhoNovaImagem = await UparImagem(arquivo, nomeFoto, GetDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), string.Empty, _webHostEnvironment);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

            static string GerarNomeFoto(int usuarioId)
            {
                return $"{usuarioId}{GerarStringAleatoria(5, true)}.jpg";
            }
        }
    }
}