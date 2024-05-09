using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using TimeZoneConverter;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Encrypt;

namespace Wards.Utils.Fixtures
{
    public static class Get
    {
        /// <summary>
        /// Pegar informações do appsettings;
        /// stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        /// </summary>
        static readonly IConfigurationRoot builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        static readonly string _urlFrontDev = builder.GetSection("URLSettings")["FrontDev"] ?? string.Empty;
        static readonly string _urlFrontProd = builder.GetSection("URLSettings")["FrontProd"] ?? string.Empty;

        /// <summary>
        /// Obtém o horário atual, forçando ao horário de Brasilia;
        /// </summary>
        public static DateTime GerarHorarioBrasilia()
        {
            TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        }

        /// <summary>
        /// Gera um texto Lorem Ipsum;
        /// </summary>
        public static string GerarLoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs, bool isAdicionarTagP)
        {
            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new();
            for (int p = 0; p < numParagraphs; p++)
            {
                if (isAdicionarTagP)
                {
                    result.Append("<p>");
                }

                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0)
                        {
                            result.Append(' ');
                        }

                        result.Append(words[rand.Next(words.Length)]);
                    }

                    result.Append(". ");
                }

                if (isAdicionarTagP)
                {
                    result.Append("</p>");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Obtém a descrição de um enum;
        /// </summary>
        public static string ObterDescricaoEnum(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute? attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);

            return attribute!.Description;
        }

        /// <summary>
        /// Gera um número aleatório com base na em um valor mínimo e máximo;
        /// </summary>
        public static int GerarNumeroAleatorio(int min, int max)
        {
            Random random = new();
            int numeroAleatorio = random.Next(min, max + 1);

            return numeroAleatorio;
        }

        /// <summary>
        /// Gera uma string aleatória com base na quantidade de caracteres desejados;
        /// </summary>
        public static string GerarStringAleatoria(int qtdCaracteres, bool isApenasMaiusculas)
        {
            Random random = new();
            string caracteres = (isApenasMaiusculas ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            string? stringAleatoria = new(Enumerable.Repeat(caracteres, qtdCaracteres).Select(s => s[random.Next(s.Length)]).ToArray());

            return stringAleatoria;
        }

        /// <summary>
        /// Gera um código hash para o usuário com base no usuarioId + string aleatória;
        /// </summary>
        public static string GerarHashUsuario(int usuarioId)
        {
            string palavraAleatoria = $"{usuarioId}_{GerarStringAleatoria(GerarNumeroAleatorio(10, 15), false)}";
            string hash = Criptografar(palavraAleatoria).Replace("/", string.Empty);

            return hash;
        }

        /// <summary>
        /// Verifica se a aplicação está sendo executada em localhost ou publicada;
        /// </summary>
        public static bool ObterIsDebug()
        {
#if DEBUG
            return true;
#else
        return false;
#endif
        }

        /// <summary>
        /// Verifica se o front-end está sendo executado em localhost ou publicado;
        /// </summary>
        public static string ObterCaminhoFront()
        {
            if (ObterIsDebug())
            {
                return _urlFrontDev;
            }

            return _urlFrontProd;
        }

        /// <summary>
        /// Detalha em texto a data e hora atual;
        /// </summary>
        public static string ObterDetalhesDataHora()
        {
            DateTime horarioBrasilia = GerarHorarioBrasilia();
            return $"{horarioBrasilia:dd/MM/yyyy} às {horarioBrasilia:HH:mm:ss}";
        }

        /// <summary>
        /// Clona um objeto de forma que não há problemas em suas referências;
        /// Para mais informações, busque: Deep Clone;
        /// </summary>
        public static T GerarDeepClone<T>(T objeto)
        {
            if (objeto is null)
            {
                throw new Exception($"Houve uma falha interna ao gerar deep clone do objeto {objeto!.GetType()}");
            }

            var deserialize = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var clone = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objeto), deserialize)!;

            return clone;
        }

        /// <summary>
        /// Clona uma lista de objetos de forma que não há problemas em suas referências;
        /// </summary>
        public static List<T> GerarDeepCloneLista<T>(List<T> lista)
        {
            if (lista == null)
            {
                throw new ArgumentNullException(nameof(lista), "A lista não pode ser nula");
            }

            List<T> clonedList = new();

            foreach (T item in lista)
            {
                clonedList.Add(GerarDeepClone(item));
            }

            return clonedList;
        }

        /// <summary>
        /// Verifica se a propriedade é uma Key (principal);
        /// </summary>
        public static bool IsKey(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(KeyAttribute));
        }

        /// <summary>
        /// Verifica se a propriedade é uma FK;
        /// </summary>
        public static bool IsForeignKey(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var isCollection = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
            var isClass = propertyType.IsClass && propertyType != typeof(string);

            return isCollection || isClass;
        }

        /// <summary>
        /// Verifica se a propriedade é uma propriedade não-mapeada;
        /// </summary>
        public static bool IsNotMapped(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(NotMappedAttribute));
        }

        /// <summary>
        /// Detecta o encoding do arquivo (via Stream);
        /// </summary>
        public static Encoding DetectarEncoding(Stream stream)
        {
            byte[] bom = new byte[4];
            stream.Read(bom, 0, 4);

            if (bom.Length >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }
            if (bom.Length >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
            {
                if (bom.Length >= 4 && bom[2] == 0 && bom[3] == 0)
                {
                    return Encoding.UTF32;
                }
                return Encoding.Unicode;
            }
            if (bom.Length >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            {
                return Encoding.UTF8;
            }

            return Encoding.UTF8;
        }

        /// <summary>
        /// Recebe uma lista e o número de chunks que essa lista deve ser splitada em chunks;
        /// </summary>
        public static List<List<T>> SepararListaEmChunks<T>(List<T> source, int numeroDeChunks)
        {
            if (numeroDeChunks <= 0)
            {
                throw new ArgumentException("O número de chunks deve ser maior que 0.", nameof(numeroDeChunks));
            }

            int chunkSize = (int)Math.Ceiling((double)source.Count / numeroDeChunks);
            List<List<T>> chunks = new(numeroDeChunks);

            for (int i = 0; i < numeroDeChunks; i++)
            {
                int startIndex = i * chunkSize;
                int endIndex = Math.Min((i + 1) * chunkSize, source.Count);
                chunks.Add(source.GetRange(startIndex, endIndex - startIndex));
            }

            return chunks;
        }

        /// <summary>
        /// Verifica se um serviço em questão, passado por parâmetro, está ou não instalado na máquina;
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validar a compatibilidade da plataforma", Justification = "<Pendente>")]
        public static bool IsServicoInstaladoNaMaquina(string servico)
        {
            try
            {
                ServiceController[]? listaServicos = ServiceController.GetServices();
                bool isServicoInstaladoNaMaquina = listaServicos.Any(s => s.ServiceName.Equals(servico, StringComparison.OrdinalIgnoreCase));

                return isServicoInstaladoNaMaquina;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtém o nome de um método assíncrono;
        /// </summary>
        public static string ObterNomeDoMetodo([CallerMemberName] string? nome = null)
        {
            if (string.IsNullOrEmpty(nome))
            {
                throw new Exception("Houve um problema ao obter o nome do método requisitado");
            }

            return nome ?? string.Empty;
        }

        /// <summary>
        /// Obtém todo restante de uma string após certo delimitador informado por parâmetro;
        /// </summary>
        public static string ObterStringAposDelimitador(string? str, char delimitador, bool isEstourarException = true)
        {
            if (str is null)
            {
                return string.Empty;
            }

            int index = str.LastIndexOf(delimitador);

            if (index != -1)
            {
                return str[(index + 1)..];
            }

            if (isEstourarException)
            {
                throw new Exception($"Falha ao executar o método {ObterNomeDoMetodo()}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Recebe um <Enum> e lista todos os valores dele mapeados pela classe de resposta "EnumOutput";
        /// O método trata o Enum caso ele tenha/não tenha objetos com "[Description]";
        /// </summary>
        public static List<EnumOutput> ListarEnum<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).
                   Cast<TEnum>().
                   Select(x =>
                   {
                       FieldInfo? info = x.GetType().GetField(x.ToString());
                       string desc = info!.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute ? descriptionAttribute.Description : x.ToString();
                       return new EnumOutput { Id = (int)(object)x, Item = info.Name, Desc = desc };
                   }).ToList();
        }

        /// <summary>
        /// Obtém a extensão de um arquivo IFormFile;
        /// </summary>
        public static string ObterExtensao(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return string.Empty;
            }

            string nome = file.FileName;
            string extensao = Path.GetExtension(nome);

            return extensao;
        }

        /// <summary>
        /// Obtém de forma segura o Environment.ProcessorCount, caso o parâmetro ultrapasse o limite, defina o resultado como metade da capacidade total;
        /// </summary>
        public static int ObterNumeroDeThreadsSafeMode(int numeroThreadAlvo)
        {
            if (numeroThreadAlvo >= Environment.ProcessorCount)
            {
                int numeroThreadSafe = Environment.ProcessorCount / 2;
                return numeroThreadSafe < 1 ? 1 : numeroThreadSafe;
            }

            return numeroThreadAlvo;
        }
    }
}