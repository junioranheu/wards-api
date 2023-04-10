using Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands;

namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado
{
    public class DeletarFeriadoEstadoUseCase : IDeletarFeriadoEstadoUseCase
    {
        private readonly IDeletarFeriadoEstadoCommand _deletarFeriadoEstadoCommand;

        public DeletarFeriadoEstadoUseCase(IDeletarFeriadoEstadoCommand deletarFeriadoEstadoCommand)
        {
            _deletarFeriadoEstadoCommand = deletarFeriadoEstadoCommand;
        }

        public async Task Execute(int feriadoId)
        {
            await _deletarFeriadoEstadoCommand.Execute(feriadoId);
        }
    }
}