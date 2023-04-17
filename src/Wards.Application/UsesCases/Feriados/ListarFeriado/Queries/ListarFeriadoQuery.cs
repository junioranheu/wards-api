﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Feriados.ListarFeriado.Queries
{
    public class ListarFeriadoQuery : IListarFeriadoQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarFeriadoQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Feriado>> Execute()
        {
            try
            {
                var linq = await _context.Feriados.
                                  Include(u => u.Usuarios).
                                  Include(um => um.UsuariosMods).
                                  Include(fd => fd.FeriadosDatas).
                                  Include(fe => fe.FeriadosEstados)!.ThenInclude(e => e.Estados).
                                  ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}