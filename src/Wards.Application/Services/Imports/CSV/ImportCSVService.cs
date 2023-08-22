using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text;
using Wards.Application.Services.Imports.Shared.Models.Input;
using static Wards.Utils.Fixtures.Get;

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

            string csv = LerCSV(input.FormFile);
            List<object?> listaObjetoFinal = IterarConteudo(csv, input);
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

        private static List<object?> IterarConteudo(string csv, ImportCSVInput input)
        {
            string[] linhas = csv.Split('\n');

            Type tipo = input.ClasseAlvo!.GetType();
            PropertyInfo[] propriedades = tipo.GetProperties();

            List<object?> listaObjetoFinal = new();

            int i = 0;
            foreach (var linha in linhas)
            {
                if (input.IsPularCabecalho && i == 0)
                {
                    i++;
                    continue;
                }

                string[] dados = linha.Split(input.Separador);
                object novaInstanciaObjetoAlvo = Activator.CreateInstance(tipo)!;

                int j = 0;
                foreach (var prop in propriedades)
                {
                    if (j >= dados.Length)
                    {
                        break;
                    }

                    bool isKeyProperty = IsKey(prop);
                    bool isForeignKey = IsForeignKey(prop);
                    bool isNotMapped = IsNotMapped(prop);

                    if (!prop.CanWrite || isKeyProperty || isForeignKey || isNotMapped)
                    {
                        continue;
                    }

                    object? dadoNormalizado = NormalizarDado(dados[j], prop.PropertyType);
                    prop.SetValue(novaInstanciaObjetoAlvo, dadoNormalizado);
                    j++;
                }

                listaObjetoFinal.Add(novaInstanciaObjetoAlvo);
                i++;
            }

            return listaObjetoFinal;
        }

        private static object? NormalizarDado(object dado, Type tipo)
        {
            if (dado is null)
            {
                // Retornar um valor padrão para tipos por valor (int, double, etc.)
                if (tipo.IsValueType)
                {
                    return Activator.CreateInstance(tipo)!;
                }

                // Se o tipo for uma classe, apenas retorna null
                return null;
            }

            // Convertendo o objeto "dado" para o tipo especificado;
            object resultado = Convert.ChangeType(dado, tipo);

            return resultado;
        }
    }
}