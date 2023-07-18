using AutoMapper;
using Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro.Commands;
using Wards.Application.UseCases.NewslettersCadastros.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro
{
    public sealed class CriarNewsletterCadastroUseCase : ICriarNewsletterCadastroUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarNewsletterCadastroCommand _criarCommand;

        public CriarNewsletterCadastroUseCase(IMapper map, ICriarNewsletterCadastroCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task<int> Execute(NewsletterCadastroInput input)
        {
            return await _criarCommand.Execute(_map.Map<NewsletterCadastro>(input));
        }
    }
}