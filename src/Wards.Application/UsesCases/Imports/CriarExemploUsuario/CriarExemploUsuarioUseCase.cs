using Microsoft.AspNetCore.Http;
using System.Data;
using Wards.Application.Services.Import.CSV.Importar;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Imports.CriarExemploUsuario
{
    public sealed class CriarExemploUsuarioUseCase : ICriarExemploUsuarioUseCase
    {
        public readonly IImportService _importHandler;

        public CriarExemploUsuarioUseCase(IImportService importHandler)
        {
            _importHandler = importHandler;
        }

        public async Task<(DataTable?, bool)> Execute(IFormFile formFile, int justificativaId)
        {
            return await _importHandler.InserirCsv("CsvImportExemploUsuarios", new CsvImportExemploUsuario(), formFile, justificativaId, isVerificarData: false, nomesEquipamentos: new List<string>());
        }
    }
}