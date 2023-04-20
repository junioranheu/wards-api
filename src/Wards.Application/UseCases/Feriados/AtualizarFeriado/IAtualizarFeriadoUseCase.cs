using Wards.Application.UseCases.Feriados.Shared.Models.Input;

namespace Wards.Application.UseCases.CriarFeriados.AtualizarFeriado
{
    public interface IAtualizarFeriadoUseCase
    {
        Task<int> Execute(FeriadoInput input);
    }
}