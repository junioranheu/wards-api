using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Wards.ListarWard.Queries
{
    public sealed class ListarWardQuery : IListarWardQuery
    {
        private readonly WardsContext _context;

        public ListarWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> Execute(PaginacaoInput input, string keyword)
        {
            var linq = await _context.Wards.
                       Include(u => u.Usuarios).
                       Include(u => u.UsuariosMods).
                       Include(wh => wh.WardsHashtags)!.ThenInclude(h => h.Hashtags).
                       Where(w =>
                          w.IsAtivo == true &&
                          (!string.IsNullOrEmpty(keyword) ? (w.Titulo.Contains(keyword) || w.Conteudo.Contains(keyword)) || w.WardsHashtags!.Any(x => x.Hashtags!.Tag.Contains(keyword)) : true)
                       ).
                       OrderByDescending(w => w.DataMod).ThenByDescending(w => w.Data).
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}