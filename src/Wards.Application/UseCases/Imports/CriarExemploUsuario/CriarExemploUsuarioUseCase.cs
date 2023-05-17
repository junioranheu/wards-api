using Microsoft.AspNetCore.Http;
using System.Data;
using Wards.Application.Services.Imports.CSV.Importar;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Imports.CriarExemploUsuario
{
    public sealed class CriarExemploUsuarioUseCase : ICriarExemploUsuarioUseCase
    {
        public readonly IImportCsvService _importHandler;

        public CriarExemploUsuarioUseCase(IImportCsvService importHandler)
        {
            _importHandler = importHandler;
        }

        public async Task<(DataTable? tabelaErros, bool isErroBanco)> Execute(IFormFile formFile, int justificativaId)
        {
            return await _importHandler.InserirCsv("CsvImportExemploUsuarios", new CsvImportExemploUsuario(), formFile, justificativaId, isVerificarData: false, nomesEquipamentos: new List<string>());
        }
    }
}