using Microsoft.AspNetCore.Http;
using System.Text;
using Wards.Application.Services.Imports.Shared.Models.Input;

namespace Wards.Application.Services.Imports.CSV
{
    public sealed class ImportCSVService : IImportCSVService
    {
        public async Task ImportarCSV(ImportCSVInput input)
        {
            if (input.FormFile is null || input.FormFile.Length == 0)
            {
                throw new Exception("O arquivo a ser importado é inválido e/ou está corrompido");
            }

            string teste = LerCSV(input.FormFile);
        }

        private static string LerCSV(IFormFile formFile)
        {
            Encoding encoding;
            using (var stream = formFile.OpenReadStream())
            {
                encoding = DetectarEncoding(stream);
                stream.Close();
            }

            if (encoding != Encoding.UTF8 && encoding != Encoding.Latin1)
                throw new Exception("Encoding inválido");

            using MemoryStream memoryStream = new();
            formFile.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using StreamReader reader = new(memoryStream, encoding);
            string csvContent = reader.ReadToEnd();

            return csvContent;
        }

        private static Encoding DetectarEncoding(Stream stream)
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
    }
}