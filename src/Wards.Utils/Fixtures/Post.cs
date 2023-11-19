using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Convert;

namespace Wards.Utils.Fixtures
{
    public static class Post
    {
        /// <summary>
        /// Pega as informações do appsettings;
        /// stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        /// </summary>
        static readonly IConfigurationRoot builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        static readonly string _emailDominio = builder.GetSection("EmailSettings")["Domain"] ?? string.Empty;
        static readonly string _emailPorta = builder.GetSection("EmailSettings")["Port"] ?? string.Empty;
        static readonly string _emailEmail = builder.GetSection("EmailSettings")["Email"] ?? string.Empty;
        static readonly string _emailChave = builder.GetSection("EmailSettings")["Key"] ?? string.Empty;
        static readonly string _emailRemetente = builder.GetSection("EmailSettings")["Name"] ?? string.Empty;

        /// <summary>
        /// Envia um e-mail (SMTP) via Gmail;
        /// www.youtube.com/watch?v=FZfneLNyE4o&ab_channel=AWPLife 
        /// </summary>
        public static async Task<bool> EnviarEmail(string emailTo, string assunto, string nomeArquivo, List<EmailDadosReplaceOutput> listaDadosReplace)
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

            static string AjustarConteudoEmailHTML(string caminhoFinalArquivoHTML, List<EmailDadosReplaceOutput>? listaDadosReplace)
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
        /// nomeArquivoSemExtensao = o nome do novo objeto em questão;
        /// extensao = por exemplo: ".jpg";
        /// path = nome do caminho do arquivo, da pasta. Por exemplo: "/Uploads/Usuarios/";
        /// nomeArquivoAnterior = o nome do arquivo que constava anterior, caso exista;
        /// hostingEnvironment = o caminho até o wwwroot;
        /// </summary>
        public static async Task<string?> SubirArquivoEmPasta(IFormFile arquivo, string nomeArquivoSemExtensao, string extensao, string path, string? nomeArquivoAnteriorSemExtensao, string webRootPath)
        {
            if (arquivo is null || arquivo.Length <= 0)
            {
                throw new Exception("O arquivo é inválido pois parece ser nulo");
            }

            string fullPath = Path.Combine(webRootPath, path);
            string nomeArquivoComExtensao = $"{nomeArquivoSemExtensao}{extensao}";
            string fullPathComExtensao = Path.Combine(webRootPath, path, nomeArquivoComExtensao);

            if (!Path.HasExtension(fullPathComExtensao))
            {
                throw new Exception("O arquivo é inválido pois não contém uma extensão");
            }

            if (!Directory.Exists(fullPath))
            {
                throw new Exception("Diretório inválido");
            }

            // Verificar se já existe um arquivo anterior. Caso exista, delete-o;
            if (!string.IsNullOrEmpty(nomeArquivoAnteriorSemExtensao))
            {
                string pathCaminhoAnterior = Path.Combine(fullPath, $"{nomeArquivoAnteriorSemExtensao}{extensao}");

                if (File.Exists(pathCaminhoAnterior))
                {
                    File.Delete(pathCaminhoAnterior);
                }
            }

            try
            {
                using FileStream fs = File.Create(fullPathComExtensao);
                await arquivo.CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve um erro durante o processo de criação de arquivo no servidor: {ex.Message}");
            }

            return nomeArquivoComExtensao;
        }

        /// <summary>
        /// Deleta os arquivos de uma pasta;
        /// 
        /// - Todos os arquivos;
        /// DeletarArquivosEmPasta(path);
        ///
        /// - Extensões específicas;
        /// List<string> extensoesEspecificas = [".txt", ".mp4"];
        /// DeletarArquivosEmPasta(path, listaExtensoes: extensoesEspecificas);
        ///
        /// - Nomes específicos;
        /// List<string> nomesEspeficos = ["ola", "tmr_pes"];
        /// DeletarArquivosEmPasta(path, listaNomes: nomesEspeficos);
        /// </summary>
        public static bool DeletarArquivosEmPasta(string path, string webRootPath, List<string>? listaExtensoes = null, List<string>? listaNomes = null, bool? ignorarExtensaoEmListaNomes = true)
        {
            string fullPath = Path.Combine(webRootPath, path);

            if (Directory.Exists(fullPath))
            {
                string[] files;

                if (listaExtensoes is not null && listaExtensoes.Count > 0)
                {
                    files = Directory.GetFiles(fullPath).Where(x => listaExtensoes.Any(extensao => x.EndsWith(extensao, StringComparison.OrdinalIgnoreCase))).ToArray();
                }
                else if (listaNomes is not null && listaNomes.Count > 0)
                {
                    if (ignorarExtensaoEmListaNomes.GetValueOrDefault())
                    {
                        files = Directory.GetFiles(fullPath).Where(file => listaNomes.Contains(Path.GetFileNameWithoutExtension(file), StringComparer.OrdinalIgnoreCase)).ToArray();
                    }
                    else
                    {
                        files = Directory.GetFiles(fullPath).Where(x => listaNomes.Contains(Path.GetFileName(x), StringComparer.OrdinalIgnoreCase)).ToArray();
                    }
                }
                else
                {
                    files = Directory.GetFiles(fullPath);
                }

                foreach (string file in files)
                {
                    File.Delete(file);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Exemplo de streaming de um arquivo dividido em chunks;
        /// 
        /// Sites para testar/validar os chunks gerados:
        /// Base64 to .mp4: base64.guru/converter/decode/video;
        /// Base64 to .jpg: onlinejpgtools.com/convert-base64-to-jpg;
        /// </summary>
        public static async IAsyncEnumerable<StreamingFileOutput> StreamFileEmChunks(string arquivo, long chunkSizeBytes, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (arquivo is null || chunkSizeBytes < 1)
            {
                throw new Exception("Os parâmetros 'nomeArquivo' e 'chunkSizeBytes' não devem ser nulos");
            }

            Stream? stream = await ConverterPathParaStream(arquivo, chunkSizeBytes) ?? throw new Exception("Houve um erro interno ao buscar arquivo no servidor e convertê-lo em Stream");
            byte[]? buffer = new byte[chunkSizeBytes];

            while (!cancellationToken.IsCancellationRequested && (await stream.ReadAsync(buffer, cancellationToken) > 0))
            {
                // await Task.Delay(1000, cancellationToken);
                byte[]? chunk = new byte[chunkSizeBytes];

                try
                {
                    buffer.CopyTo(chunk, 0);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Houve um erro interno. Mais informações: {ex.Message}");
                }

                yield return new StreamingFileOutput()
                {
                    PorcentagemCompleta = System.Convert.ToDouble(stream.Position) / System.Convert.ToDouble(stream.Length) * 100,
                    Chunk = chunk
                };
            }
        }
    }
}