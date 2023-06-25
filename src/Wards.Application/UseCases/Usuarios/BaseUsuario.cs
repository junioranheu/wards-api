using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Wards.Application.UseCases.Tokens.CriarRefreshToken;
using Wards.Application.UseCases.Tokens.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Convert;
using static Wards.Utils.Fixtures.Get;
using static Wards.Utils.Fixtures.Post;

namespace Wards.Application.UseCases.Usuarios
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
                string nomeArquivo = ObterDescricaoEnum(EmailEnum.VerificarConta);

                List<EmailDadosReplaceOutput> listaDadosReplace = new()
                {
                    new EmailDadosReplaceOutput { Key = "Url", Value = $"{ObterCaminhoFront()}/usuario/verificar-conta/{codigoVerificacao}"},
                    new EmailDadosReplaceOutput { Key = "NomeUsuario", Value = nomeUsuario }
                };

                bool resposta = await EnviarEmail(emailTo, assunto, nomeArquivo, listaDadosReplace);
                return resposta;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static IFormFile? ObterFotoAleatoria(IWebHostEnvironment _webHostEnvironment)
        {
            string path = _webHostEnvironment.ContentRootPath;

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string caminho = $"{path}/{ObterDescricaoEnum(CaminhoAssetEnum.FotoPerfilUsuario)}";
            string[] arquivos = Directory.GetFiles(caminho, "*.jpg");

            if (arquivos.Length == 0)
            {
                return null;
            }

            string arquivoAleatorio = arquivos.OrderBy(x => Guid.NewGuid()).FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(arquivoAleatorio))
            {
                return null;
            }

            return ConverterPathParaFile(arquivoAleatorio, Path.GetFileName(arquivoAleatorio), "image/jpg");
        }

        internal static async Task<string?> VerificarParametrosDepoisUparFoto(IWebHostEnvironment _webHostEnvironment, int usuarioId, IFormFile? arquivo)
        {
            try
            {
                string nomeFoto = GerarNomeFoto(usuarioId);

                if (string.IsNullOrEmpty(nomeFoto) || arquivo is null)
                {
                    return string.Empty;
                }

                return await UparImagem(arquivo, nomeFoto, ObterDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), string.Empty, _webHostEnvironment.ContentRootPath); ;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            static string GerarNomeFoto(int usuarioId)
            {
                return $"{usuarioId}{GerarStringAleatoria(5, true)}.jpg";
            }
        }
    }
}