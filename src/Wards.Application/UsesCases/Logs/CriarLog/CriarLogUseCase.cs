using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public sealed class CriarLogUseCase : ICriarLogUseCase
    {
        public readonly ICriarLogCommand _criarLogCommand;

        public CriarLogUseCase(ICriarLogCommand criarLogCommand)
        {
            _criarLogCommand = criarLogCommand;
        }

        public async Task<int> Criar(Log input)
        {
            return await _criarLogCommand.Criar(input);
        }
    }
}
