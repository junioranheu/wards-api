using AutoMapper;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.CriarWard
{
    public sealed class CriarWardUseCase : ICriarWardUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarWardCommand _criarCommand;
        private readonly ICriarWardHashtagUseCase _criarWardHashtagUseCase;

        public CriarWardUseCase(
            IMapper map,
            ICriarWardCommand criarCommand,
            ICriarWardHashtagUseCase criarWardHashtagUseCase)
        {
            _map = map;
            _criarCommand = criarCommand;
            _criarWardHashtagUseCase = criarWardHashtagUseCase;
        }

        public async Task<int> Execute(WardInput input)
        {
            var output = await _criarCommand.Execute(_map.Map<Ward>(input));

            if (!input.ListaHashtags!.Any() || input.ListaHashtags is null)
            {
                return output;
            }

            await _criarWardHashtagUseCase.Execute(input.ListaHashtags, output);

            return output;
        }
    }
}