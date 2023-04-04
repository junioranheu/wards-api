using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.AtualizarFeriado.Commands
{
    public interface IAtualizarFeriadoCommand
    {
        Task<int> ExecuteAsync(Feriado input);
    }
}
