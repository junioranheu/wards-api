using Wards.Application.Services.Sistemas.ResetarBancoDados.Commands;

namespace Wards.Application.Services.Sistemas.ResetarBancoDados
{
    public sealed class ResetarBancoDadosService : IResetarBancoDadosService
    {
        private readonly IResetarBancoDadosCommand _resetarBancoDadosCommand;

        public ResetarBancoDadosService(IResetarBancoDadosCommand resetarBancoDadosCommand)
        {
            _resetarBancoDadosCommand = resetarBancoDadosCommand;
        }

        public async Task<bool> Execute()
        {
            return await _resetarBancoDadosCommand.Execute();
        }
    }
}