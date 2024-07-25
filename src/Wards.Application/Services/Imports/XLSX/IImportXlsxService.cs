using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Imports.XLSX;

public interface IImportXlsxService
{
    List<T> ReadExcel<T>(IFormFile? file, int sheetIndex = 0, int skipRow = 1, bool cleanEmptyItems = true, bool includeEmptyOrNullCells = false) where T : new();
}