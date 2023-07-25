using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.Wards.ObterAleatorioWard.Queries
{
    public sealed class ObterAleatorioWardQuery : IObterAleatorioWardQuery
    {
        private readonly WardsContext _context;

        public ObterAleatorioWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<Ward?> Execute()
        {
            Random rand = new();
            int max = await _context.Wards.Where(w => w.IsAtivo == true).CountAsync();

            if (max < 1)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            int skip = rand.Next(0, max);

            var linq = await _context.Wards.
                       Include(u => u.Usuarios).
                       Include(u => u.UsuariosMods).
                       Include(wh => wh.WardsHashtags)!.ThenInclude(h => h.Hashtags).
                       Where(w => w.IsAtivo == true).
                       Skip(skip).
                       Take(1).
                       AsNoTracking().FirstOrDefaultAsync();

            return linq;
        }
    }
}