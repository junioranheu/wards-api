using Microsoft.AspNetCore.Http;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Utils.Fixtures
{
    public static class Convert
    {
        /// <summary>
        /// Converter IFormFile para bytes[];
        /// https://stackoverflow.com/questions/36432028/how-to-convert-a-file-into-byte-array-in-memory;
        /// </summary>
        public static async Task<byte[]> ConverterIFormFileParaBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Converter bytes[] para IFormFile;
        /// </summary>
        public static IFormFile ConverterBytesParaIFormFile(byte[] bytes)
        {
            using var memoryStream = new MemoryStream(bytes);
            string strRandom = GerarStringAleatoria(5, false);
            FormFile formFile = new(memoryStream, 0, bytes.Length, strRandom, strRandom)
            {
                Headers = new HeaderDictionary()
            };

            return formFile;
        }

        /// <summary>
        /// Converter Base64 para arquivo;
        /// </summary>
        public static IFormFile ConverterBase64ParaFile(string base64)
        {
            List<IFormFile> formFiles = new();
            string split = ";base64,";
            string normalizarBase64 = base64;

            if (base64.Contains(split))
            {
                normalizarBase64 = base64[(base64.IndexOf(split) + split.Length)..];
            }

            byte[] bytes = System.Convert.FromBase64String(normalizarBase64);
            MemoryStream stream = new(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            formFiles.Add(file);

            return formFiles[0];
        }

        /// <summary>
        /// Converter bytes[] para Base64;
        /// </summary>
        public static string ConverterBytesParaBase64(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converter Base64 para imagem;
        /// </summary>
        public static IFormFile ConverterBase64ParaImagem(string base64)
        {
            List<IFormFile> formFiles = new();

            string split = ";base64,";
            string normalizarBase64 = base64[(base64.IndexOf(split) + split.Length)..];
            byte[] bytes = System.Convert.FromBase64String(normalizarBase64);
            MemoryStream stream = new(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            formFiles.Add(file);

            return formFiles[0];
        }

        /// <summary>
        /// Converter path de um arquivo para arquivo com base em "tipoConteudo";
        /// </summary>
        public static IFormFile ConverterPathParaFile(string path, string nomeArquivo, string tipoConteudo)
        {
            FileStream? fileStream = null;
            FormFile? formFile = null;

            try
            {
                fileStream = new FileStream(path, FileMode.Open);

                formFile = new(fileStream, 0, fileStream.Length, nomeArquivo, nomeArquivo)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = tipoConteudo
                };
            }
            finally
            {
                fileStream?.Dispose();
            }

            return formFile;
        }

        /// <summary>
        /// Converter caminho de um arquivo para stream;
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
        /// Normalizar valor que é lido por um "SqlDataReader", que muitas vezes vem quebrado;
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