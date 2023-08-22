using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;
using TimeZoneConverter;
using static Wards.Utils.Fixtures.Encrypt;

namespace Wards.Utils.Fixtures
{
    public static class Get
    {
        /// <summary>
        /// Pegar informações do appsettings;
        /// stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        /// </summary>
        static readonly string _urlFrontDev = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontDev"] ?? string.Empty;
        static readonly string _urlFrontProd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontProd"] ?? string.Empty;

        /// <summary>
        /// Converter para o horário de Brasilia;
        /// blog.yowko.com/timezoneinfo-time-zone-id-not-found/;
        /// </summary>
        public static DateTime GerarHorarioBrasilia()
        {
            TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        }

        /// <summary>
        /// Gerar Lorem Ipsum;
        /// stackoverflow.com/questions/4286487/is-there-any-lorem-ipsum-generator-in-c;
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
        /// Pegar a descrição de um enum;
        /// https://stackoverflow.com/questions/50433909/get-string-name-from-enum-in-c-sharp;
        /// </summary>
        public static string ObterDescricaoEnum(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute? attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);

            return attribute!.Description;
        }

        /// <summary>
        /// Gerar um número aleatório com base na em um valor mínimo e máximo;
        /// </summary>
        public static int GerarNumeroAleatorio(int min, int max)
        {
            Random random = new();
            int numeroAleatorio = random.Next(min, max - 1);

            return numeroAleatorio;
        }

        /// <summary>
        /// Gerar uma string aleatória com base na quantidade de caracteres desejados;
        /// </summary>
        public static string GerarStringAleatoria(int qtdCaracteres, bool isApenasMaiusculas)
        {
            Random random = new();
            string caracteres = (isApenasMaiusculas ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            string? stringAleatoria = new(Enumerable.Repeat(caracteres, qtdCaracteres).Select(s => s[random.Next(s.Length)]).ToArray());

            return stringAleatoria;
        }

        /// <summary>
        /// Gerar um código hash para o usuário com base no usuarioId + string aleatória;
        /// </summary>
        public static string GerarHashUsuario(int usuarioId)
        {
            string palavraAleatoria = $"{usuarioId}_{GerarStringAleatoria(GerarNumeroAleatorio(10, 15), false)}";
            string hash = Criptografar(palavraAleatoria).Replace("/", string.Empty);

            return hash;
        }

        /// <summary>
        /// Verificar se a aplicação está sendo executada em localhost ou publicada;
        /// </summary>
        public static bool ObterIsDebug()
        {
            // https://stackoverflow.com/questions/12135854/best-way-to-tell-if-in-production-or-development-environment-in-net
#if DEBUG
            return true;
#else
        return false;
#endif
        }

        /// <summary>
        /// Verificar se o front-end está sendo executado em localhost ou publicado;
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
        /// Para mais informações busque Deep Clone;
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
    }
}