using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Wards.Application.Services.Imports.XLSX;

/// <summary>
/// A classe tem o intuito de ser mais dinâmica possível;
/// Aceita arquivos com as extensões .xlsx e .xls;
/// </summary>
/// 
/// <example>
/// Exemplo de uso:
/// <code>
/// List<ExampleOutput>? excel = _importXlsxService.ReadExcel<ExampleOutput>(file: input.FormFile, skipRow: 1);
/// </code>
/// </example>
public sealed partial class ImportXlsxService : IImportXlsxService
{
    public List<T> ReadExcel<T>(IFormFile? file, int sheetIndex = 0, int skipRow = 1, bool cleanEmptyItems = true, bool includeEmptyOrNullCells = false) where T : new()
    {
        if (file is null || (!file!.FileName.EndsWith(".xlsx") && !file!.FileName.EndsWith(".xls")))
        {
            throw new Exception("Arquivo a ser importado está em um formato inválido");
        }

        var result = new List<T>();

        if (file is null || file?.Length == 0)
        {
            return result;
        }

        using var stream = new MemoryStream();
        file!.CopyTo(stream);
        var properties = typeof(T).GetProperties();

        // Caso o arquivo seja XLS em vez de XLSX, deve haver todo um tratamento especial para normalizá-lo para o resto do código;
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

            var rows = worksheetPart?.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Row>();

            foreach (var row in rows!.Skip(skipRow)) // Skip cabeçalho;
            {
                var item = new T();

                // Caso o parâmetro includeEmptyOrNullCells seja true, é necessário todo uma nova tratativa para obter todas as celulas do arquivo, incluindo as vazias/nulas;
                Cell[] cells = includeEmptyOrNullCells ? GetAllCellsIncludingEmptyOrNull(row) : row.Elements<Cell>().ToArray();

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

    // Obter todas as celulas incluindo as vazias;
    private static Cell[] GetAllCellsIncludingEmptyOrNull(DocumentFormat.OpenXml.Spreadsheet.Row row)
    {
        List<Cell> allCells = new();

        // Obter todas as celulas (NÃO incluindo as vazias);
        var existingCells = row.Elements<Cell>();
        allCells.AddRange(existingCells);

        // Determinar o máximo da iteração com base nas células existentes;
        int maxColumnIndex = 0;

        foreach (var cell in existingCells)
        {
            int columnIndex = GetColumnIndexFromName(GetColumnName(cell.CellReference!));
            maxColumnIndex = Math.Max(maxColumnIndex, columnIndex);
        }

        // Iterar para corrigir as possíveis faltas das celulas;
        for (int columnIndex = 1; columnIndex <= maxColumnIndex; columnIndex++)
        {
            string columnName = GetColumnNameFromIndex(columnIndex);
            string cellReference = $"{columnName}{row.RowIndex}";

            // Caso necessário, crie essa celula faltante;
            if (!existingCells.Any(cell => cell.CellReference == cellReference))
            {
                Cell newCell = new() { CellReference = cellReference, DataType = CellValues.String };
                string defaultValue = string.Empty;
                newCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(defaultValue);
                allCells.Add(newCell);
            }
        }

        Cell[] cells = allCells.OrderBy(cell => GetNumericCellReference(cell.CellReference!)).ToList().ToArray();

        return cells;
    }

    // Obter referência númerica com base na referência da celula;
    private static int GetNumericCellReference(string referenciaCelula)
    {
        string nomeColuna = GetColumnName(referenciaCelula);
        int rowIndex = int.Parse(referenciaCelula.Substring(nomeColuna.Length));
        int columnIndex = GetColumnIndexFromName(nomeColuna);

        return (rowIndex * 1000) + columnIndex;
    }

    // Obter o index da coluna pelo nome (A -> 1, B -> 2, etc);
    private static int GetColumnIndexFromName(string nomeColuna)
    {
        int index = 0;
        int mul = 1;

        for (int i = nomeColuna.Length - 1; i >= 0; i--)
        {
            index += (nomeColuna[i] - 'A' + 1) * mul;
            mul *= 26;
        }

        return index;
    }

    // Obter o nome da coluna pelo index (1 -> A, 2 -> B, etc);
    private static string GetColumnNameFromIndex(int indexColuna)
    {
        string nomeColuna = "";

        while (indexColuna > 0)
        {
            int remainder = (indexColuna - 1) % 26;
            nomeColuna = Convert.ToChar('A' + remainder) + nomeColuna;
            indexColuna = (indexColuna - remainder) / 26;
        }

        return nomeColuna;
    }

    // Extrair o nome da coluna com base na referência da celula ("A1" -> "A", "B10" -> "B", etc);
    private static string GetColumnName(string referenciaCelula)
    {
        return new string(referenciaCelula.TakeWhile(char.IsLetter).ToArray());
    }

    private static bool IsXlsFile(string arquivo)
    {
        return arquivo.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);
    }

    private static MemoryStream ConvertXlsToXlsx(MemoryStream xlsStream)
    {
        try
        {
            // Ler o XLS e convertê-lo para HSSFWorkbook;
            HSSFWorkbook hssfWorkbook;

            using (var xlsReader = new MemoryStream(xlsStream.ToArray()))
            {
                hssfWorkbook = new HSSFWorkbook(xlsReader);
            }

            var xssfWorkbook = new XSSFWorkbook();

            // Iterar arquivo para copiar dados;
            for (var i = 0; i < hssfWorkbook.NumberOfSheets; i++)
            {
                var hssfSheet = hssfWorkbook.GetSheetAt(i);
                var xssfSheet = xssfWorkbook.CreateSheet(hssfSheet.SheetName);

                // Copiar dados;
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
                                        // Em certos casos, uma data como 25/03/1997 é transformada para um número int;
                                        // É necessário realizar essa verificação para tratar caso o número na real seja uma data;
                                        if (DateUtil.IsCellDateFormatted(hssfCell))
                                        {
                                            xssfCell.SetCellValue((DateTime)hssfCell.DateCellValue!);
                                            break;
                                        }

                                        // Caso normal númerico;
                                        xssfCell.SetCellValue(hssfCell.NumericCellValue);
                                        break;
                                    case NPOI.SS.UserModel.CellType.String:
                                        string str = hssfCell.StringCellValue;

                                        // Caso a string esteja no formato "2025-Mar-03", normalize-a antes de setá-la;
                                        if (IsMonthFormat(str))
                                        {
                                            str = NormalizeMonthFormat_yyyy_MM_dd(str);
                                        }

                                        xssfCell.SetCellValue(str);
                                        break;
                                    default:
                                        try
                                        {
                                            xssfCell.SetCellValue(hssfCell.StringCellValue);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Erro SetCellValue: {ex.Message}");
                                            xssfCell.SetBlank();
                                        }

                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // Salvar o XSSFWorkbook como MemoryStream;
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

    private static string NormalizeMonthFormat_yyyy_MM_dd(string input)
    {
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

        // Extrair as partes do formato de data;
        Match match = RegexFormatoData().Match(input);

        if (match.Success)
        {
            // Obter as partes da data;
            string year = match.Groups[1].Value;
            string month = match.Groups[2].Value;
            string day = match.Groups[3].Value;

            // Traduzir o nome do mês (reverso);
            if (reverseMonthMappings.TryGetValue(month, out string? translatedMonth))
            {
                // Retornar a data normalizada no formato desejado;
                return $"{year}-{translatedMonth}-{day}";
            }
        }

        // Retornar a string original se não for possível normalizar;
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
        var invalidCharsRegex = RegexPropNome();
        var cleanedName = invalidCharsRegex.Replace(property.Name, "_");

        return cleanedName.ToLower();
    }

    private static void SetPropertyValue<T>(T item, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property != null)
        {
            // Lidar com objetos nullables;
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
            {
                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                if (underlyingType != null)
                {
                    var convertedValue = ConvertToType(value, property.PropertyType);
                    property.SetValue(item, convertedValue);
                }
            }
            // Lidar com objetos não nullables;
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
                    // A conversão de data pode ser bem complicada;
                    // Portanto, foi implementada algumas validações previamente previstas para mitigar possíveis problemas;
                    // #1 - Data no formato "padrão";
                    if (DateTime.TryParse(value?.ToString(), out DateTime result))
                    {
                        return result;
                    }
                    // #2 - Data no formato "2025-Mar-03";
                    else if (DateTime.TryParse(NormalizeMonthFormat_yyyy_MM_dd(value?.ToString()!), out DateTime result_yyyy_MM_dd))
                    {
                        return result_yyyy_MM_dd;
                    }
                    else
                    {
                        // #3 - Data no formato OA;
                        try
                        {
                            double OA = Convert.ToDouble(value);
                            DateTime oaDateTime = DateTime.FromOADate(OA);

                            return oaDateTime;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return null;
                        }
                    }
                }
                else
                {
                    return Convert.ChangeType(value, targetType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

    [GeneratedRegex("^(\\d{4})-([A-Za-z]{3})-(\\d{2})$")]
    private static partial Regex RegexFormatoData();

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex RegexPropNome();
}