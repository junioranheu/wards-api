using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro.Queries
{
    public sealed class ListarNewsletterCadastroQuery : IListarNewsletterCadastroQuery
    {
        private readonly WardsContext _context;

        public ListarNewsletterCadastroQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsletterCadastro>> Execute(PaginacaoInput input)
        {
            var linq = await _context.NewslettersCadastros.
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}