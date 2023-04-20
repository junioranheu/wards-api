using AutoMapper;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.CriarWard
{
    public sealed class CriarWardUseCase : ICriarWardUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarWardCommand _criarCommand;

        public CriarWardUseCase(IMapper map, ICriarWardCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task<int> Execute(WardInput input)
        {
            return await _criarCommand.Execute(_map.Map<Ward>(input));
        }
    }
}