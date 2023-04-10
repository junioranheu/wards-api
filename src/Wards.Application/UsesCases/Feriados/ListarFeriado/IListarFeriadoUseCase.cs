using Wards.Application.UsesCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.Feriados.ListarFeriado
{
    public interface IListarFeriadoUseCase
    {
        Task<IEnumerable<FeriadoOutput>> Execute();
    }
}