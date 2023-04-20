using Wards.Application.UseCases.Feriados.Shared.Models.Input;

namespace Wards.Application.UseCases.CriarFeriados.CriarFeriado
{
    public interface ICriarFeriadoUseCase
    {
        Task<int> Execute(FeriadoInput input);
    }
}
