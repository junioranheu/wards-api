using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Wards.Application.Services.GenericReadExcel
{
    /// <summary>
    /// GenericReadExcel funciona - no momento - apenas para arquivos .XLSX;
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
                //// Converter XLS para XLSX;
                //stream.Seek(0, SeekOrigin.Begin);
                //var xlsxStream = ConverterXlsParaXlsx(stream);
                //stream.Dispose();
                //stream.SetLength(0);
                //xlsxStream.CopyTo(stream);
                //xlsxStream.Dispose();
                //stream.Seek(0, SeekOrigin.Begin);
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
    }
}