using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.API.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected BaseController() { }

        protected string ObterUsuarioEmail()
        {
            if (User.Identity!.IsAuthenticated)
            {
                // Obter o e-mail do usuário pela Azure;
                // var claim = User.Claims.First(c => c.Type == "preferred_username");
                // return claim.Value ?? string.Empty;

                // Obter o e-mail do usuário pela autenticação própria;
                string email = User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }

        protected async Task<int> ObterUsuarioId()
        {
            var service = HttpContext.RequestServices.GetService<IObterUsuarioCacheService>();
            UsuarioOutput? usuario = await service!.Execute(ObterUsuarioEmail());

            return usuario is not null ? usuario.UsuarioId : 0;
        }

        /// <summary>
        /// arquivo = o arquivo em si, a variável IFormFile;
        /// nomeArquivo = o nome do novo objeto em questão. Por exemplo, ao mudar a foto de perfil de um usuário, envie o id dele;
        /// nomePasta = nome do caminho do arquivo, da pasta. Por exemplo: /Uploads/Usuarios/. "Usuarios" é o caminho;
        /// nomeArquivoAnterior = o nome do arquivo que constava anterior, caso exista;
        /// hostingEnvironment = o caminho até o wwwroot. Ele deve ser passado por parâmetro, já que não funcionaria aqui diretamente no BaseController;
        /// </summary>
        protected async Task<string> UparImagem(IFormFile arquivo, string nomeArquivo, string nomePasta, string? nomeArquivoAnterior, IWebHostEnvironment hostingEnvironment)
        {
            return await Task.Run(() =>
            {
                // Procedimento de inicialização para salvar nova imagem;
                string webRootPath = hostingEnvironment.ContentRootPath; // Vai até o wwwwroot;
                string restoCaminho = $"/Uploads/{nomePasta}/"; // Acesso à pasta referente; 

                // Verificar se o arquivo tem extensão, se não tiver, adicione;
                if (!Path.HasExtension(nomeArquivo))
                {
                    nomeArquivo = $"{nomeArquivo}.webp";
                }

                string caminhoDestino = webRootPath + restoCaminho + nomeArquivo; // Caminho de destino para upar;

                // Copiar o novo arquivo para o local de destino;
                if (arquivo.Length > 0)
                {
                    // Verificar se já existe uma foto caso exista, delete-a;
                    if (!String.IsNullOrEmpty(nomeArquivoAnterior))
                    {
                        string caminhoArquivoAtual = webRootPath + restoCaminho + nomeArquivoAnterior;

                        // Verificar se o arquivo existe;
                        if (System.IO.File.Exists(caminhoArquivoAtual))
                        {
                            // Se existe, apague-o; 
                            System.IO.File.Delete(caminhoArquivoAtual);
                        }
                    }

                    // Então salve a imagem no servidor no formato WebP - https://blog.elmah.io/convert-images-to-webp-with-asp-net-core-better-than-png-jpg-files/;
                    using (var webPFileStream = new FileStream(caminhoDestino, FileMode.Create))
                    {
                        ImageFactory imageFactory = new(preserveExifData: false);
                        imageFactory.Load(arquivo.OpenReadStream())
                                    .Format(new WebPFormat())
                                    .Quality(10)
                                    .Save(webPFileStream);
                    }

                    return nomeArquivo;
                }
                else
                {
                    return "";
                }
            });
        }
    }
}