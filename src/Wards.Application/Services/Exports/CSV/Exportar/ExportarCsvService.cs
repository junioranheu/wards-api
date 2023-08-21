using System.Reflection;

namespace Wards.Application.Services.Exports.CSV.Exportar
{
    public sealed class ExportCSVService : IExportCSVService
    {
        public byte[]? ConverterDadosParaCSVEmBytes<T>(List<T>? lista, string[,] colunas)
        {
            byte[]? bytes = null;

            if (!lista!.Any() || lista is null)
                return bytes;

            using (var ms = new MemoryStream())
            {
                using TextWriter writer = new StreamWriter(ms);

                GerarColunas(colunas, writer);
                GerarConteudo(lista, colunas, writer);

                writer.Flush();
                ms.Position = 0;
                bytes = ms.ToArray();
            }

            return bytes;
        }

        private static void GerarColunas(string[,] colunas, TextWriter writer)
        {
            List<string> nomeColunas = new();

            for (int i = 0; i < colunas.GetLength(0); i++)
            {
                nomeColunas.Add(colunas[i, 0]);
            }

            writer.WriteLine(string.Join(";", nomeColunas));
        }

        private static void GerarConteudo<T>(List<T>? lista, string[,] colunas, TextWriter writer)
        {
            foreach (var item in lista!)
            {
                dynamic? itemFinal = null;

                for (int i = 0; i < colunas.GetLength(0); i++)
                {
                    string colunaAtual = colunas[i, 1];
                    PropertyInfo? reflection = item!.GetType().GetProperty(colunaAtual);
                    object? valor = reflection!.GetValue(item, null);

                    itemFinal += $"{valor};";
                }

                writer.WriteLine(itemFinal?.TrimEnd(';'));
            }
        }
    }
}