﻿using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Wards.ObterWard.Queries
{
    public sealed class ObterWardQuery : IObterWardQuery
    {
        private readonly WardsContext _context;

        public ObterWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<Ward?> Execute(int id)
        {
            var linq = await _context.Wards.
                       Include(u => u.Usuarios).
                       Include(u => u.UsuariosMods).
                       Include(wh => wh.WardsHashtags)!.ThenInclude(h => h.Hashtags).
                       Where(w => w.WardId == id && w.IsAtivo == true).
                       AsNoTracking().FirstOrDefaultAsync();

            return linq;
        }
    }
}