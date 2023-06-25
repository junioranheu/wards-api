using Wards.Application.UseCases.Feriados.Shared.Models.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.Feriados.ListarFeriado
{
    public interface IListarFeriadoUseCase
    {
        Task<IEnumerable<FeriadoOutput>> Execute(PaginacaoInput input);
    }
}