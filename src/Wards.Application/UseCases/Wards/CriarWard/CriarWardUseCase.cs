using AutoMapper;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.CriarWard
{
    public sealed class CriarWardUseCase : ICriarWardUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarWardCommand _criarCommand;
        private readonly ICriarWardHashtagCommand _criarWardHashtagCommand;

        public CriarWardUseCase(
            IMapper map,
            ICriarWardCommand criarCommand,
            ICriarWardHashtagCommand criarWardHashtagCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
            _criarWardHashtagCommand = criarWardHashtagCommand;
        }

        public async Task<int> Execute(WardInput input)
        {
            var output = await _criarCommand.Execute(_map.Map<Ward>(input));

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