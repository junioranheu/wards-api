using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Wards.Application.Services.GenericReadExcel
{
    /// <summary>
    /// GenericReadExcel aceita arquivos .XLSX e .XLS;
    /// </summary>
    public sealed partial class GenericReadExcel
    {
        public static List<T> ReadExcel<T>(IFormFile? file, int sheetIndex = 0, int skipRow = 1, bool cleanEmptyItems = true) where T : new()
        {
            var result = new List<T>();

            if (file is null || file?.Length == 0)
            {
                return result;
            }

            using var stream = new MemoryStream();
            file!.CopyTo(stream);
            var properties = typeof(T).GetProperties();

            if (IsXlsFile(file.FileName))
            {
                stream.Seek(0, SeekOrigin.Begin);

                using MemoryStream xlsxStreamConvert = ConvertXlsToXlsx(stream);

                if (xlsxStreamConvert.Length > 0)
                {
                    stream.SetLength(0);
                    xlsxStreamConvert.WriteTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            if (stream.Length <= 0)
            {
                throw new InvalidOperationException("O arquivo está vazio.");
            }

            using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                var sheet = workbookPart?.Workbook.Descendants<Sheet>().ElementAt(sheetIndex);
                var worksheetPart = (WorksheetPart)workbookPart?.GetPartById(sheet?.Id!)!;

                var rows = worksheetPart?.Worksheet.Descendants<Row>();

                foreach (var row in rows!.Skip(skipRow)) // Skip cabeçalho;
                {
                    var item = new T();
                    var cells = row.Elements<Cell>().ToArray();

                    for (int i = 0; i < cells.Length; i++)
                    {
                        var cellValue = GetCellValue(workbookPart!, cells[i]);

                        if (i >= properties.Length)
                        {
                            continue;
                        }

                        var propertyName = GetPropertyName(properties[i]);
                        SetPropertyValue(item, propertyName, cellValue);
                    }

                    result.Add(item);
                }
            }

            if (!cleanEmptyItems)
            {
                return result;
            }

            foreach (var item in result.ToList())
            {
                bool isAllEmpty = CheckAreAllPropertiesNullOrDefault(item);

                if (!isAllEmpty)
                {
                    continue;
                }

                result.Remove(item);
            }

            return result;
        }
        private static bool IsXlsFile(string arquivo)
        {
            return arquivo.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);
        }

        private static MemoryStream ConvertXlsToXlsx(MemoryStream xlsStream)
        {
            try
            {
                // Read the XLS file into an HSSFWorkbook;
                HSSFWorkbook hssfWorkbook;

                using (var xlsReader = new MemoryStream(xlsStream.ToArray()))
                {
                    hssfWorkbook = new HSSFWorkbook(xlsReader);
                }

                // Create a new XSSFWorkbook;
                var xssfWorkbook = new XSSFWorkbook();

                // Iterate through sheets and copy data;
                for (var i = 0; i < hssfWorkbook.NumberOfSheets; i++)
                {
                    var hssfSheet = hssfWorkbook.GetSheetAt(i);
                    var xssfSheet = xssfWorkbook.CreateSheet(hssfSheet.SheetName);

                    // Copy data;
                    for (var rowIndex = 0; rowIndex <= hssfSheet.LastRowNum; rowIndex++)
                    {
                        var hssfRow = hssfSheet.GetRow(rowIndex);
                        var xssfRow = xssfSheet.CreateRow(rowIndex);

                        if (hssfRow != null)
                        {
                            for (var cellIndex = 0; cellIndex < hssfRow.LastCellNum; cellIndex++)
                            {
                                var hssfCell = hssfRow.GetCell(cellIndex);
                                var xssfCell = xssfRow.CreateCell(cellIndex);

                                if (hssfCell != null)
                                {
                                    switch (hssfCell.CellType)
                                    {
                                        case NPOI.SS.UserModel.CellType.Numeric:
                                            xssfCell.SetCellValue(hssfCell.NumericCellValue);
                                            break;
                                        case NPOI.SS.UserModel.CellType.String:
                                            string str = hssfCell.StringCellValue;

                                            if (IsMonthFormat(str))
                                            {
                                                str = NormalizeMonthFormat(str);
                                            }

                                            xssfCell.SetCellValue(str);
                                            break;
                                        default:
                                            xssfCell.SetCellValue(hssfCell.StringCellValue);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Save the XSSFWorkbook to a MemoryStream;
                var xlsxMemoryStream = new MemoryStream();
                xssfWorkbook.Write(xlsxMemoryStream, leaveOpen: true);

                return xlsxMemoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao converter arquvo XLS para XLSX: {ex.Message}");
                throw;
            }
        }

        private static bool IsMonthFormat(string input)
        {
            string pattern = @"^\d{4}-[A-Za-z]{3}-\d{2}$";
            return Regex.IsMatch(input, pattern);
        }

        private static string NormalizeMonthFormat(string input)
        {
            // Dicionário para mapear nomes dos meses de inglês para português;
            var monthMappings = new Dictionary<string, string>
            {
                {"Jan", "Jan"},
                {"Feb", "Fev"},
                {"Mar", "Mar"},
                {"Apr", "Abr"},
                {"May", "Mai"},
                {"Jun", "Jun"},
                {"Jul", "Jul"},
                {"Aug", "Ago"},
                {"Sep", "Set"},
                {"Oct", "Out"},
                {"Nov", "Nov"},
                {"Dec", "Dez"}
            };

            // Dicionário para mapear nomes dos meses de português para inglês (reverso);
            var reverseMonthMappings = new Dictionary<string, string>();
            foreach (var entry in monthMappings)
            {
                reverseMonthMappings[entry.Value] = entry.Key;
            }

            // Extrai as partes do formato de data;
            Match match = RegexData().Match(input);

            if (match.Success)
            {
                // Obtém as partes da data;
                string year = match.Groups[1].Value;
                string month = match.Groups[2].Value;
                string day = match.Groups[3].Value;

                // Traduz o nome do mês (reverso)
                if (reverseMonthMappings.TryGetValue(month, out string? translatedMonth))
                {
                    // Retorna a data normalizada no formato desejado;
                    return $"{year}-{translatedMonth}-{day}";
                }
            }

            // Retorna a string original se não for possível normalizar;
            return input;
        }

        private static string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTablePart = workbookPart.SharedStringTablePart;

                if (sharedStringTablePart != null)
                {
                    var sharedStringTable = sharedStringTablePart.SharedStringTable;
                    return sharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
                }
            }

            return cell.InnerText;
        }

        private static string GetPropertyName(PropertyInfo property)
        {
            var invalidCharsRegex = RegexPropertyName();
            var cleanedName = invalidCharsRegex.Replace(property.Name, "_");

            return cleanedName.ToLower();
        }

        private static void SetPropertyValue<T>(T item, string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                // Handle nullable;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
                {
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                    if (underlyingType != null)
                    {
                        var convertedValue = ConvertToType(value, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                }
                // Handle non-nullable;
                else
                {
                    if (value != null)
                    {
                        var convertedValue = ConvertToType(value, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                }
            }

            static object? ConvertToType(object value, Type targetType)
            {
                try
                {
                    if (targetType == typeof(double) || targetType == typeof(double?))
                    {
                        if (string.IsNullOrEmpty((string?)value))
                        {
                            value = 0;
                        }

                        return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    }
                    else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value?.ToString(), out DateTime result))
                        {
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return Convert.ChangeType(value, targetType);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static bool CheckAreAllPropertiesNullOrDefault<T>(T obj)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(obj);

                if (!IsNullOrDefault(value))
                {
                    return false;
                }
            }

            return true;

            static bool IsNullOrDefault(object? value)
            {
                if (value == null)
                {
                    return true;
                }

                Type valueType = value.GetType();

                if (valueType == typeof(string))
                {
                    return string.IsNullOrEmpty((string)value);
                }
                else if (valueType == typeof(DateTime))
                {
                    return (DateTime)value == DateTime.MinValue;
                }
                else if (valueType == typeof(byte))
                {
                    return (byte)value == 0;
                }
                else if (IsNumericType(valueType))
                {
                    return Convert.ToDouble(value) == 0;
                }

                return false;
            }

            static bool IsNumericType(Type type)
            {
                return type.IsPrimitive && type != typeof(char) && type != typeof(bool) && type != typeof(nint) && type != typeof(nuint);
            }
        }

        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex RegexPropertyName();

        [GeneratedRegex("^(\\d{4})-([A-Za-z]{3})-(\\d{2})$")]
        private static partial Regex RegexData();
    }
}