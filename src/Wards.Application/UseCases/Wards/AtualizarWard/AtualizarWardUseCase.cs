using AutoMapper;
using Wards.Application.UseCases.Wards.AtualizarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.AtualizarWard
{
    public sealed class AtualizarWardUseCase : IAtualizarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IAtualizarWardCommand _atualizarCommand;
        private readonly ICriarWardHashtagCommand _criarWardHashtagCommand;

        public AtualizarWardUseCase(
            IMapper map,
            IAtualizarWardCommand atualizarCommand,
            ICriarWardHashtagCommand criarWardHashtagCommand)
        {
            _map = map;
            _atualizarCommand = atualizarCommand;
            _criarWardHashtagCommand = criarWardHashtagCommand;
        }

        public async Task<int> Execute(WardInput input)
        {
            var output = await _atualizarCommand.Execute(_map.Map<Ward>(input));

            if (!input.ListaHashtags!.Any() || input.ListaHashtags is null)
            {
                return output;
            }

            List<WardHashtag> listaHashtag = new();

            foreach (var item in input.ListaHashtags)
            {
                WardHashtag hashtag = new()
                {
                    WardId = output,
                    HashtagId = item
                };

                listaHashtag.Add(hashtag);
            }

            if (listaHashtag.Any())
            {
                await _criarWardHashtagCommand.Execute(listaHashtag, output);
            }

            return output;
        }
    }
}