using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole.Commands
{
    public sealed class CriarUsuarioRoleCommand : ICriarUsuarioRoleCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarUsuarioRoleCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(int[] rolesId, int usuarioId)
        {
            try
            {
                List<UsuarioRole> listUp = new();

                for (int i = 0; i < rolesId?.Length; i++)
                {
                    UsuarioRole up = new()
                    {
                        UsuarioId = usuarioId,
                        RoleId = (UsuarioRoleEnum)rolesId[i]
                    };

                    listUp.Add(up);
                }

                await _context.AddRangeAsync(listUp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}