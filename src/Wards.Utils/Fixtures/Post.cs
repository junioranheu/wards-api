using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Wards.Utils.Entities;

namespace Wards.Utils.Fixtures
{
    public static class Post
    {
        /// <summary>
        /// Pegar informações do appsettings;
        /// stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        /// </summary>
        static readonly string _emailDominio = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Domain"] ?? string.Empty;
        static readonly string _emailPorta = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Port"] ?? string.Empty;
        static readonly string _emailEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Email"] ?? string.Empty;
        static readonly string _emailChave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Key"] ?? string.Empty;
        static readonly string _emailRemetente = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Name"] ?? string.Empty;

        /// <summary>
        /// SMTP Gmail;
        /// https://www.youtube.com/watch?v=FZfneLNyE4o&ab_channel=AWPLife 
        /// </summary>
        public static async Task<bool> EnviarEmail(string emailTo, string assunto, string nomeArquivo, List<EmailDadosReplace> listaDadosReplace)
        {
            if (string.IsNullOrEmpty(emailTo) || string.IsNullOrEmpty(assunto) || string.IsNullOrEmpty(nomeArquivo))
            {
                return false;
            }

            string caminhoFinalArquivoHTML = $"{Directory.GetCurrentDirectory()}/Emails/{nomeArquivo}";
            string conteudoEmailHTML = AjustarConteudoEmailHTML(caminhoFinalArquivoHTML, listaDadosReplace);

            try
            {
                MailMessage mail = new()
                {
                    From = new MailAddress(_emailEmail, _emailRemetente)
                };

                mail.To.Add(new MailAddress(emailTo));
                // mail.CC.Add(new MailAddress(emailTo));

                mail.Subject = assunto;
                mail.Body = conteudoEmailHTML;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                // mail.Attachments.Add(new Attachment(arquivo));

                using SmtpClient smtp = new(_emailDominio, System.Convert.ToInt32(_emailPorta));
                smtp.Credentials = new NetworkCredential(_emailEmail, _emailChave);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

            static string AjustarConteudoEmailHTML(string caminhoFinalArquivoHTML, List<EmailDadosReplace>? listaDadosReplace)
            {
                string conteudoEmailHtml = string.Empty;

                using (var reader = new StreamReader(caminhoFinalArquivoHTML))
                {
                    // #1 - Ler arquivo;
                    string readFile = reader.ReadToEnd();
                    string strContent = readFile;

                    // #2 - Remover tags desnecessárias;
                    strContent = strContent.Replace("\r", string.Empty);
                    strContent = strContent.Replace("\n", string.Empty);

                    // #3 - Replaces utilizando o parâmetro "listaDadosReplace";[
                    if (listaDadosReplace?.Count > 0)
                    {
                        foreach (var item in listaDadosReplace)
                        {
                            strContent = strContent.Replace($"[{item.Key}]", item.Value);
                        }
                    }

                    // #4 - Gerar resultado final;
                    conteudoEmailHtml = strContent.ToString();
                }

                return conteudoEmailHtml;
            }
        }

        /// <summary>
        /// arquivo = o arquivo em si, a variável IFormFile;
        /// nomeArquivo = o nome do novo objeto em questão;
        /// nomePasta = nome do caminho do arquivo, da pasta. Por exemplo: /Uploads/Usuarios/. "Usuarios" é o caminho;
        /// nomeArquivoAnterior = o nome do arquivo que constava anterior, caso exista;
        /// hostingEnvironment = o caminho até o wwwroot. Ele deve ser passado por parâmetro, já que não funcionaria aqui diretamente no BaseController;
        /// </summary>
        public static async Task<string?> UparImagem(IFormFile arquivo, string nomeArquivo, string nomePasta, string? nomeArquivoAnterior, string webRootPath)
        {
            if (arquivo.Length <= 0)
            {
                return string.Empty;
            }

            // Procedimento de inicialização para salvar nova imagem;
            string restoCaminho = $"/{nomePasta}/"; // Acesso à pasta referente; 

            // Verificar se o arquivo tem extensão, se não tiver, adicione;
            if (!Path.HasExtension(nomeArquivo))
            {
                nomeArquivo = $"{nomeArquivo}.jpg";
            }

            // Verificar se já existe uma foto caso exista, delete-a;
            if (!string.IsNullOrEmpty(nomeArquivoAnterior))
            {
                string caminhoArquivoAtual = webRootPath + restoCaminho + nomeArquivoAnterior;

                // Verificar se o arquivo existe;
                if (System.IO.File.Exists(caminhoArquivoAtual))
                {
                    System.IO.File.Delete(caminhoArquivoAtual); // Se existe, apague-o; 
                }
            }

            // Salvar aquivo;
            string caminhoDestino = webRootPath + restoCaminho + nomeArquivo; // Caminho de destino para upar;
            using (FileStream fs = File.Create(caminhoDestino))
            {
                await arquivo.CopyToAsync(fs);
            }

            return nomeArquivo;
        }
    }
}