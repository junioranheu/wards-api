using Wards.Application.UsesCases.Feriados.Shared.Models.Input;

namespace Wards.Application.UseCases.CriarFeriados.CriarFeriado
{
    public interface ICriarFeriadoUseCase
    {
        Task<int> ExecuteAsync(FeriadoInput input);
    }
}
