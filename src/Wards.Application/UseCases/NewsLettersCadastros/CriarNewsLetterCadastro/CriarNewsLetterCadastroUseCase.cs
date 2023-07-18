using AutoMapper;
using Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro.Commands;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro
{
    public sealed class CriarNewsLetterCadastroUseCase : ICriarNewsLetterCadastroUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarNewsLetterCadastroCommand _criarCommand;

        public CriarNewsLetterCadastroUseCase(IMapper map, ICriarNewsLetterCadastroCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task<int> Execute(NewsLetterCadastroInput input)
        {
            return await _criarCommand.Execute(_map.Map<NewsLetterCadastro>(input));
        }
    }
}