using Wards.Application.UsesCases.Feriados.Shared.Models.Output;
using Wards.Application.UsesCases.Shared.Models;

namespace Wards.Application.UseCases.Feriados.ListarFeriado
{
    public interface IListarFeriadoUseCase
    {
        Task<IEnumerable<FeriadoOutput>> Execute(PaginacaoInput input);
    }
}