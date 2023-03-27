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
        /// nomeArquivo = o nome do novo objeto em questão;
        /// nomePasta = nome do caminho do arquivo, da pasta. Por exemplo: /Uploads/Usuarios/. "Usuarios" é o caminho;
        /// nomeArquivoAnterior = o nome do arquivo que constava anterior, caso exista;
        /// hostingEnvironment = o caminho até o wwwroot. Ele deve ser passado por parâmetro, já que não funcionaria aqui diretamente no BaseController;
        /// </summary>
        protected async Task<string> UparImagem(IFormFile arquivo, string nomeArquivo, string nomePasta, string? nomeArquivoAnterior, IWebHostEnvironment hostingEnvironment)
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
                string caminhoArquivoAtual = Path.Combine(webRootPath, restoCaminho, nomeArquivoAnterior);

                // Verificar se o arquivo existe;
                if (System.IO.File.Exists(caminhoArquivoAtual))
                    System.IO.File.Delete(caminhoArquivoAtual); // Se existe, apague-o; 
            }

            // Salvar aquivo;
            string caminhoDestino = Path.Combine(webRootPath, restoCaminho, nomeArquivo); // Caminho de destino para upar;
            await arquivo.CopyToAsync(new FileStream(caminhoDestino, FileMode.Create));

            return nomeArquivo;
        }
    }
}