using AutoMapper;
using Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands;
using Wards.Application.UseCases.WardsHashtags.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.WardsHashtags.CriarWardHashtag
{
    public sealed class CriarWardHashtagUseCase : ICriarWardHashtagUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarWardHashtagCommand _criarCommand;

        public CriarWardHashtagUseCase(IMapper map, ICriarWardHashtagCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task Execute(List<WardHashtagInput> listaInput, int wardId)
        {
            await _criarCommand.Execute(_map.Map<List<WardHashtag>>(listaInput), wardId);
        }

        public async Task Execute(int[] listaHashtags, int wardId)
        {
            List<WardHashtag> listaHashtag = new();

            foreach (var item in listaHashtags)
            {
                WardHashtag hashtag = new()
                {
                    WardId = wardId,
                    HashtagId = item
                };

                listaHashtag.Add(hashtag);
            }

            if (listaHashtag.Any())
            {
                await _criarCommand.Execute(_map.Map<List<WardHashtag>>(listaHashtag), wardId);
            }
        }
    }
}