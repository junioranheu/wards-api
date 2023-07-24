using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.ObterAleatorioWard.Queries
{
    public interface IObterAleatorioWardQuery
    {
        Task<Ward?> Execute();
    }
}