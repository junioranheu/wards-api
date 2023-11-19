using Microsoft.AspNetCore.Http;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Utils.Fixtures
{
    public static class Convert
    {
        /// <summary>
        /// Converte IFormFile para bytes[];
        /// </summary>
        public static async Task<byte[]> ConverterIFormFileParaBytes(IFormFile formFile)
        {
            MemoryStream memoryStream = new();
            memoryStream.Seek(0, SeekOrigin.Begin);
            await formFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Converte IFormFile para Base64;
        /// </summary>
        public static string ConverterIFormFileParaBase64(IFormFile formFile)
        {
            MemoryStream memoryStream = new();
            memoryStream.Seek(0, SeekOrigin.Begin);
            formFile.CopyTo(memoryStream);

            byte[] fileBytes = memoryStream.ToArray();
            string base64String = System.Convert.ToBase64String(fileBytes);

            return base64String;
        }

        /// <summary>
        /// Converte Base64 para arquivo;
        /// </summary>
        public static IFormFile ConverterBase64ParaIFormFile(string base64)
        {
            string split = ";base64,";
            string normalizarBase64 = base64;

            if (base64.Contains(split))
            {
                normalizarBase64 = base64[(base64.IndexOf(split) + split.Length)..];
            }

            byte[] bytes = System.Convert.FromBase64String(normalizarBase64);
            MemoryStream memoryStream = new(bytes);
            memoryStream.Seek(0, SeekOrigin.Begin);

            IFormFile file = new FormFile(memoryStream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            return file;
        }

        /// <summary>
        /// Converte Base64 para bytes[];
        /// </summary>
        public static byte[] ConverterBase64ParaBytes(string base64)
        {
            return System.Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Converte bytes[] para IFormFile;
        /// </summary>
        public static IFormFile ConverterBytesParaIFormFile(byte[] bytes)
        {
            MemoryStream memoryStream = new (bytes);
            memoryStream.Seek(0, SeekOrigin.Begin);
            string strRandom = GerarStringAleatoria(5, false); 

            FormFile formFile = new(memoryStream, 0, bytes.Length, strRandom, strRandom)
            {
                Headers = new HeaderDictionary()
            };

            return formFile;
        }

        /// <summary>
        /// Converte bytes[] para Base64;
        /// </summary>
        public static string ConverterBytesParaBase64(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converte path de um arquivo para arquivo com base em "tipoConteudo";
        /// </summary>
        public static IFormFile ConverterPathParaIFormFile(string path, string nomeArquivo, string tipoConteudo)
        {
            FileStream? fileStream = new(path, FileMode.Open);

            FormFile? formFile = new(fileStream, 0, fileStream.Length, nomeArquivo, nomeArquivo)
            {
                Headers = new HeaderDictionary(),
                ContentType = tipoConteudo
            };

            return formFile;
        }

        /// <summary>
        /// Converte caminho de um arquivo para stream;
        /// </summary>
        public static async Task<Stream?> ConverterPathParaStream(string path, long? chunkSize = 4096)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            return await Task.FromResult(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, (int)chunkSize.GetValueOrDefault(), FileOptions.Asynchronous));
        }

        /// <summary>
        /// Auto-sugestivo;
        /// </summary>
        public static int ConverterMegasParaBytes(double? megas)
        {
            return System.Convert.ToInt32(megas.GetValueOrDefault() * (1024 * 1024));
        }

        /// <summary>
        /// Normaliza valor que é lido por um "SqlDataReader", que muitas vezes vem quebrado;
        /// stackoverflow.com/a/870771;
        /// </summary>
        public static T? NormalizarSqlDataReader<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default;
            }

            return (T)obj;
        }
    }
}