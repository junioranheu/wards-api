﻿using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole.Commands
{
    public sealed class CriarUsuarioRoleCommand : ICriarUsuarioRoleCommand
    {
        private readonly WardsContext _context;

        public CriarUsuarioRoleCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Criar(int[] rolesId, int usuarioId)
        {
            List<UsuarioRole> listUp = new();

            for (int i = 0; i < rolesId?.Length; i++)
            {
                UsuarioRole up = new()
                {
                    UsuarioId = usuarioId,
                    RoleId = rolesId[i]
                };

                listUp.Add(up);
            }

            await _context.AddRangeAsync(listUp);
            await _context.SaveChangesAsync();
        }
    }
}