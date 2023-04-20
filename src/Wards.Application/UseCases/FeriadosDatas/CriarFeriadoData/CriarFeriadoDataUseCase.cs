using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands;

namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData
{
    public class CriarFeriadoDataUseCase : ICriarFeriadoDataUseCase
    {
        private readonly ICriarFeriadoDataCommand _criarFeriadoDataCommand;

        public CriarFeriadoDataUseCase(ICriarFeriadoDataCommand criarFeriadoDataCommand)
        {
            _criarFeriadoDataCommand = criarFeriadoDataCommand;
        }

        public async Task Execute(string[] data, int feriadoId)
        {
            await _criarFeriadoDataCommand.Execute(data, feriadoId);
        }
    }
}