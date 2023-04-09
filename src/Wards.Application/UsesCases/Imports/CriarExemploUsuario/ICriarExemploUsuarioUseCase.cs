using Microsoft.AspNetCore.Http;
using System.Data;

namespace Wards.Application.UsesCases.Imports.CriarExemploUsuario
{
    public interface ICriarExemploUsuarioUseCase
    {
        Task<(DataTable? tabelaErros, bool isErroBanco)> Execute(IFormFile formFile, int justificativaId);
    }
}