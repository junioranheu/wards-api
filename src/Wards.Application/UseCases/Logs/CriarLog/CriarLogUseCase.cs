using AutoMapper;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Logs.CriarLog
{
    public sealed class CriarLogUseCase : ICriarLogUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarLogCommand _criarCommand;

        public CriarLogUseCase(IMapper map, ICriarLogCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task Execute(LogInput input)
        {
            await _criarCommand.Execute(_map.Map<Log>(input));
        }
    }
}