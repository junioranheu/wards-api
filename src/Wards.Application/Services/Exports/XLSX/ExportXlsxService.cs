using ClosedXML.Excel;
using System.Reflection;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Format;

namespace Wards.Application.Services.Exports.XLSX
{
    public sealed class ExportXLSXService : IExportXLSXService
    {
        /// <summary>
        /// O parâmetro "tipoExport" é "controverso" (não foi encontrado uma maneira melhor de implementar essa feature - já que não é possível SetarValor() e MergearCelulas() fora desse service);
        /// Esse parâmetro deve ser controlado à mão... a cada nova necessidade, um if novo é criado em AdicionarRowInicial().
        /// </summary>
        public byte[]? ConverterDadosParaXLSXEmBytes<T>(List<T>? lista, string[,] colunas, string nomeSheet, bool isDataFormatoExport, string aplicarEstiloNasCelulas, TipoExportEnum? tipoExport = null)
        {
            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add(nomeSheet);

            int linhaAtual = 1; // Dependendo se há ou não "tipoExport", a "linhaAtual" é alterada (para ajustar o título e o conteúdo);

            // Estilo padrão;
            if (!string.IsNullOrEmpty(aplicarEstiloNasCelulas))
            {
                SetarEstilo(worksheet, aplicarEstiloNasCelulas, corR: 232, corG: 233, corB: 235);
            }

            // Row inicial;
            if (tipoExport is not null)
            {
                linhaAtual = AdicionarRowInicial(worksheet, tipoExport, linhaAtual);
            }

            // Título das colunas;
            for (int i = 0; i < colunas.GetLength(0); i++)
            {
                SetarValor(worksheet, linhaAtual - 1, i, typeof(string), colunas[i, 0]);
            }

            // Conteúdo;
            linhaAtual = GerarConteudo(lista, isDataFormatoExport, worksheet, colunas, linhaAtual);

            byte[]? xlsx = GerarXLSXEmBytes(workbook);

            return xlsx;
        }

        private static int AdicionarRowInicial(IXLWorksheet worksheet, TipoExportEnum? tipoExport, int linhaAtual)
        {
            if (tipoExport == TipoExportEnum.LOG)
            {
                AdicionarRowInicialLog(worksheet);
                linhaAtual = 2;
            }

            return linhaAtual;

            static void AdicionarRowInicialLog(IXLWorksheet worksheet)
            {
                SetarValor(worksheet, linhaAtual: 0, i: 0, typeof(string), "Mensagem #1"); // A;
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "A1:A1", corR: 119, corG: 147, corB: 60);

                SetarValor(worksheet, linhaAtual: 0, i: 1, typeof(string), "Mensagem #2"); // B - I;
                MergearCelulas(worksheet, linhaA: 0, colunaA: 1, linhaB: 0, colunaB: 8);
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "B1:I1", corR: 255, corG: 255, corB: 153);

                SetarValor(worksheet, linhaAtual: 0, i: 9, typeof(string), ""); // J;
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "J1:J1", corR: 204, corG: 255, corB: 255);

                SetarValor(worksheet, linhaAtual: 0, i: 10, typeof(string), "Mensagem #4"); // K - Q;
                MergearCelulas(worksheet, linhaA: 0, colunaA: 10, linhaB: 0, colunaB: 16);
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "K1:Q1", corR: 204, corG: 255, corB: 204);

                SetarValor(worksheet, linhaAtual: 0, i: 17, typeof(string), "Mensagem #5"); // R - U;
                MergearCelulas(worksheet, linhaA: 0, colunaA: 17, linhaB: 0, colunaB: 20);
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "R1:U1", corR: 51, corG: 204, corB: 204);

                SetarValor(worksheet, linhaAtual: 0, i: 21, typeof(string), ""); // V;
                SetarEstilo(worksheet, aplicarEstiloNasCelulas: "V1:V1", corR: 255, corG: 204, corB: 153);
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

                    if (valor is not null)
                    {
                        SetarValor(worksheet, linhaAtual, i, valor!.GetType(), valor);
                    }
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
            else if (tipo == typeof(int))
            {
                valorConvertido = Convert.ToInt32(valor);
            }
            else if (tipo == typeof(double))
            {
                valorConvertido = Convert.ToDouble(valor);
            }
            else
            {
                valorConvertido = Convert.ToString(valor);
            }

            worksheet.Cell(linhaAtual + 1, i + 1).SetValue(valorConvertido);
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
            worksheet.Range(worksheet.Cell(linhaA + 1, colunaA + 1), worksheet.Cell(linhaB + 1, colunaB + 1)).Merge();
        }

        private static void SetarEstilo(IXLWorksheet worksheet, string aplicarEstiloNasCelulas, int corR, int corG, int corB)
        {
            IXLStyle style = XLWorkbook.DefaultStyle;

            style.Fill.SetBackgroundColor(XLColor.FromArgb(corR, corG, corB));
            style.Font.SetBold(true);

            style.Border.BottomBorder = XLBorderStyleValues.Thin;
            style.Border.TopBorder = XLBorderStyleValues.Thin;
            style.Border.LeftBorder = XLBorderStyleValues.Thin;
            style.Border.RightBorder = XLBorderStyleValues.Thin;

            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Range(aplicarEstiloNasCelulas).Style = style;
        }
    }
}