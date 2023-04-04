using Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands;

namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData
{
    public class DeletarFeriadoDataUseCase : IDeletarFeriadoDataUseCase
    {
        private readonly IDeletarFeriadoDataCommand _deletarFeriadoDataCommand;

        public DeletarFeriadoDataUseCase(IDeletarFeriadoDataCommand deletarFeriadoDataCommand)
        {
            _deletarFeriadoDataCommand = deletarFeriadoDataCommand;
        }

        public async Task ExecuteAsync(int feriadoId)
        {
            await _deletarFeriadoDataCommand.ExecuteAsync(feriadoId);
        }
    }
}