using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public sealed class CriarLogUseCase : ICriarLogUseCase
    {
        private readonly ICriarLogCommand _criarLogCommand;

        public CriarLogUseCase(ICriarLogCommand criarLogCommand)
        {
            _criarLogCommand = criarLogCommand;
        }

        public async Task Execute(Log input)
        {
             await _criarLogCommand.Execute(input);
        }
    }
}
