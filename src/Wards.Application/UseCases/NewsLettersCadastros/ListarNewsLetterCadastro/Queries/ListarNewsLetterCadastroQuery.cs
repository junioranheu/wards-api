using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro.Queries
{
    public sealed class ListarNewsLetterCadastroQuery : IListarNewsLetterCadastroQuery
    {
        private readonly WardsContext _context;

        public ListarNewsLetterCadastroQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsLetterCadastro>> Execute(PaginacaoInput input)
        {
            var linq = await _context.NewsLettersCadastros.
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}