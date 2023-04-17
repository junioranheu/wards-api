using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries
{
    public sealed class ObterUsuarioCondicaoArbitrariaQuery : IObterUsuarioCondicaoArbitrariaQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ObterUsuarioCondicaoArbitrariaQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(Usuario? usuario, string senha)> Execute(string login)
        {
            try
            {
                var byEmail = await _context.Usuarios.
                              Where(e => e.Email == login).
                              Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                              AsNoTracking().FirstOrDefaultAsync();

                if (byEmail is null)
                {
                    var byNomeUsuario = await _context.Usuarios.
                                        Where(n => n.NomeUsuarioSistema == login).
                                        Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                        AsNoTracking().FirstOrDefaultAsync();

                    if (byNomeUsuario is null)
                    {
                        return (null, string.Empty);
                    }

                    return (byNomeUsuario, byNomeUsuario.Senha ?? string.Empty);
                }

                return (byEmail, byEmail.Senha ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}