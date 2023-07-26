using AutoMapper;
using Wards.Application.UseCases.Wards.AtualizarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.AtualizarWard
{
    public sealed class AtualizarWardUseCase : IAtualizarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IAtualizarWardCommand _atualizarCommand;
        private readonly ICriarWardHashtagUseCase _criarWardHashtagUseCase;

        public AtualizarWardUseCase(
            IMapper map,
            IAtualizarWardCommand atualizarCommand,
            ICriarWardHashtagUseCase criarWardHashtagUseCase)
        {
            _map = map;
            _atualizarCommand = atualizarCommand;
            _criarWardHashtagUseCase = criarWardHashtagUseCase;
        }

        public async Task<int> Execute(WardInput input)
        {
            var output = await _atualizarCommand.Execute(_map.Map<Ward>(input));

            if (!input.ListaHashtags!.Any() || input.ListaHashtags is null)
            {
                return output;
            }

            await _criarWardHashtagUseCase.Execute(input.ListaHashtags, output);

            return output;
        }
    }
}