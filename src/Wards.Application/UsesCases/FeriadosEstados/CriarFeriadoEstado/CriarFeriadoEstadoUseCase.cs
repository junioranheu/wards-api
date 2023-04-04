using Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands;

namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado
{
    public class CriarFeriadoEstadoUseCase : ICriarFeriadoEstadoUseCase
    {
        private readonly ICriarFeriadoEstadoCommand _criarFeriadoEstadoCommand;

        public CriarFeriadoEstadoUseCase(ICriarFeriadoEstadoCommand criarFeriadoEstadoCommand)
        {
            _criarFeriadoEstadoCommand = criarFeriadoEstadoCommand;
        }

        public async Task ExecuteAsync(int[] estadoId, int feriadoId)
        {
            await _criarFeriadoEstadoCommand.ExecuteAsync(estadoId, feriadoId);
        }
    }
}