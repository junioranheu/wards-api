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
                string nomeArquivo = ObterDescricaoEnum(EmailEnum.VerificarConta);

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

        internal static IFormFile? ObterFotoAleatoria(IWebHostEnvironment _webHostEnvironment)
        {
            string caminho = $"{_webHostEnvironment.ContentRootPath}/{ObterDescricaoEnum(CaminhoAssetEnum.FotoPerfilUsuario)}";
            string[] arquivos = Directory.GetFiles(caminho, "*.jpg");

            if (arquivos.Length == 0)
                return null;

            string arquivoAleatorio = arquivos.OrderBy(x => Guid.NewGuid()).FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(arquivoAleatorio))
                return null;

            return PathToFile(arquivoAleatorio, Path.GetFileName(arquivoAleatorio), "image/jpg");
        }

        internal static async Task<bool> VerificarParametrosDepoisUparFoto(IWebHostEnvironment _webHostEnvironment, int usuarioId, IFormFile? arquivo)
        {
            try
            {
                string nomeFoto = GerarNomeFoto(usuarioId);

                if (string.IsNullOrEmpty(nomeFoto) || arquivo is null)
                    return false;

                string? caminhoUpload = await UparImagem(arquivo, nomeFoto, ObterDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), string.Empty, _webHostEnvironment);

                return caminhoUpload?.Length > 0;
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

        /// <summary>
        /// arquivo = o arquivo em si, a variável IFormFile;
        /// nomeArquivo = o nome do novo objeto em questão;
        /// nomePasta = nome do caminho do arquivo, da pasta. Por exemplo: /Uploads/Usuarios/. "Usuarios" é o caminho;
        /// nomeArquivoAnterior = o nome do arquivo que constava anterior, caso exista;
        /// hostingEnvironment = o caminho até o wwwroot. Ele deve ser passado por parâmetro, já que não funcionaria aqui diretamente no BaseController;
        /// </summary>
        internal static async Task<string?> UparImagem(IFormFile arquivo, string nomeArquivo, string nomePasta, string? nomeArquivoAnterior, IWebHostEnvironment hostingEnvironment)
        {
            if (arquivo.Length <= 0)
                return string.Empty;

            // Procedimento de inicialização para salvar nova imagem;
            string webRootPath = hostingEnvironment.ContentRootPath; // Vai até o wwwwroot;
            string restoCaminho = $"/{nomePasta}/"; // Acesso à pasta referente; 

            // Verificar se o arquivo tem extensão, se não tiver, adicione;
            if (!Path.HasExtension(nomeArquivo))
                nomeArquivo = $"{nomeArquivo}.jpg";

            // Verificar se já existe uma foto caso exista, delete-a;
            if (!string.IsNullOrEmpty(nomeArquivoAnterior))
            {
                string caminhoArquivoAtual = webRootPath + restoCaminho + nomeArquivoAnterior;

                // Verificar se o arquivo existe;
                if (System.IO.File.Exists(caminhoArquivoAtual))
                    System.IO.File.Delete(caminhoArquivoAtual); // Se existe, apague-o; 
            }

            // Salvar aquivo;
            string caminhoDestino = webRootPath +  restoCaminho + nomeArquivo; // Caminho de destino para upar;
            await arquivo.CopyToAsync(new FileStream(caminhoDestino, FileMode.Create));

            return nomeArquivo;
        }
    }
}