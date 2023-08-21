using Wards.Application.Services.Imports.Shared.Models.Input;

namespace Wards.Application.Services.Imports.CSV
{
    public interface IImportCSVService
    {
        Task ImportarCSV(ImportCSVInput input);
    }
}