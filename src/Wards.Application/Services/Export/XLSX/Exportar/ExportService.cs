using ClosedXML.Excel;
using System.Reflection;
using static Wards.Utils.Common;

namespace Wards.Application.Services.Export.XLSX.Exportar
{
    public sealed class ExportService : IExportService
    {
        /// <summary>
        /// O parâmetro "tipoRowInicial" é "controverso" (não foi encontrado uma maneira melhor de implementar essa feature - já que não é possível SetarValor() e MergearCelulas() fora desse service);
        /// É necessário, em alguns casos, inserir uma row de "título" a mais; 
        /// Nesses casos, devem ser criados à mão (diferentemente do título principal) campo a campo;
        /// Caso o parâmetro seja menor que 1, significa que essa row a mais é descartada;
        /// Esse parâmetro deve ser controlado à mão... a cada nova necessidade, um if novo é criado em AdicionarRowInicial().
        /// </summary>
        public byte[]? ConverterDadosParaXLSXEmBytes<T>(List<T>? lista, string[,] colunas, string nomeSheet, bool isDataFormatoExport, string aplicarEstiloNasCelulas, int tipoRowInicial = 0)
        {
            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add(nomeSheet);

            // Row inicial;
            if (tipoRowInicial > 0)
                AdicionarRowInicial(worksheet, tipoRowInicial);

            if (!string.IsNullOrEmpty(aplicarEstiloNasCelulas))
            {
                IXLStyle style = XLWorkbook.DefaultStyle;
                style.Fill.SetBackgroundColor(XLColor.LightGray);
                style.Font.SetBold(true);
                style.Border.BottomBorder = XLBorderStyleValues.Thin;
                style.Border.TopBorder = XLBorderStyleValues.Thin;
                style.Border.LeftBorder = XLBorderStyleValues.Thin;
                style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Range(aplicarEstiloNasCelulas).Style = style;
            }

            int linhaAtual = tipoRowInicial > 0 ? 2 : 1; // Dependendo se há ou não "tipoRowInicial", a "linhaAtual" é alterada (para ajustar o título e o conteúdo);

            // Título das colunas;
            for (int i = 0; i < colunas.GetLength(0); i++)
            {
                SetarValor(worksheet, (linhaAtual - 1), i, typeof(String), colunas[i, 0]);
            }

            // Conteúdo;
            linhaAtual = GerarConteudo(lista, isDataFormatoExport, worksheet, colunas, linhaAtual);

            byte[]? xlsx = GerarXLSXEmBytes(workbook);

            return xlsx;
        }

        private static void AdicionarRowInicial(IXLWorksheet worksheet, int tipoRowInicial)
        {
            if (tipoRowInicial == 1)
            {
                AdicionarRowInicialConsolidadoCargaGlobal(worksheet);
            }

            static void AdicionarRowInicialConsolidadoCargaGlobal(IXLWorksheet worksheet)
            {
                SetarValor(worksheet, 0, 0, typeof(String), "Mensagem #1"); // A;

                SetarValor(worksheet, 0, 1, typeof(String), "Mensagem #2"); // B - I;
                MergearCelulas(worksheet, 0, 1, 0, 8);

                SetarValor(worksheet, 0, 9, typeof(String), ""); // J;

                SetarValor(worksheet, 0, 10, typeof(String), "Mensagem #4"); // K - Q;
                MergearCelulas(worksheet, 0, 10, 0, 16);

                SetarValor(worksheet, 0, 17, typeof(String), "Mensagem #5"); // R - U;
                MergearCelulas(worksheet, 0, 17, 0, 20);

                SetarValor(worksheet, 0, 21, typeof(String), ""); // V;
            }
        }

        private static int GerarConteudo<T>(List<T>? lista, bool isDataFormatoExport, IXLWorksheet worksheet, string[,] colunas, int linhaAtual)
        {
            foreach (var item in lista!)
            {
                for (int i = 0; i < colunas.GetLength(0); i++)
                {
                    PropertyInfo? reflection = item!.GetType().GetProperty(colunas[i, 1]);
                    object? valor = reflection!.GetValue(item, null);

                    valor = VerificarSeExisteTerceiroParametro_ObterValor_SeSim(colunas, i, valor);
                    valor = VerificarSeNecessarioFormatarAlgumDado(isDataFormatoExport, reflection, valor);

                    SetarValor(worksheet, linhaAtual, i, valor!.GetType(), valor);
                }

                linhaAtual++;
            }

            return linhaAtual;

            /// <summary>
            /// Caso exista um terceiro valor que vem do parâmetro "colunas", busque-o;
            /// </summary>
            static object? VerificarSeExisteTerceiroParametro_ObterValor_SeSim(string[,] colunas, int i, object? valor)
            {
                if (valor is not null && !string.IsNullOrEmpty(colunas[i, 2]))
                {
                    PropertyInfo? reflection2 = valor!.GetType().GetProperty(colunas[i, 2]);
                    valor = reflection2!.GetValue(valor, null);
                }

                return valor;
            }

            /// <summary>
            /// Caso exista algum tripo de tratamento para algum tipo de dado, essa é a hora;
            /// </summary>
            static object? VerificarSeNecessarioFormatarAlgumDado(bool isDataFormatoExport, PropertyInfo? reflection, object? valor)
            {
                if (isDataFormatoExport && reflection!.PropertyType == typeof(DateTime))
                {
                    valor = FormatarDataExport(Convert.ToDateTime(valor));
                }

                return valor;
            }
        }

        private static void SetarValor(IXLWorksheet worksheet, int linhaAtual, int i, Type? tipo, object? valor)
        {
            dynamic? valorConvertido;

            if (tipo == typeof(DateTime))
            {
                valorConvertido = Convert.ToDateTime(valor);
            }
            else if (tipo == typeof(Int32))
            {
                valorConvertido = Convert.ToInt32(valor);
            }
            else if (tipo == typeof(Double))
            {
                valorConvertido = Convert.ToDouble(valor);
            }
            else
            {
                valorConvertido = Convert.ToString(valor);
            }

            worksheet.Cell((linhaAtual + 1), (i + 1)).SetValue(valorConvertido);
        }

        private static byte[]? GerarXLSXEmBytes(XLWorkbook workbook)
        {
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            byte[]? xlsx = stream.ToArray();

            return xlsx;
        }

        private static void MergearCelulas(IXLWorksheet worksheet, int linhaA, int colunaA, int linhaB, int colunaB)
        {
            worksheet.Range(worksheet.Cell((linhaA + 1), (colunaA + 1)), worksheet.Cell((linhaB + 1), (colunaB + 1))).Merge();
        }
    }
}