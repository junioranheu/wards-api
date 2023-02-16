using Wards.Application.UsesCases.Logs.CriarLog.Commands;

namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public sealed class CriarLogUseCase : ICriarLogUseCase
    {
        public readonly ICriarLogCommand _criarLogCommand;

        public CriarLogUseCase(ICriarLogCommand criarLogCommand)
        {
            _criarLogCommand = criarLogCommand;
        }

        public async Task<int> ExecuteAsync(Log dto)
        {
            return await _criarLogCommand.ExecuteAsync(dto);
        }
    }
}
