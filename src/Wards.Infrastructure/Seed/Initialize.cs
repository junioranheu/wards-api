using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Infrastructure.Seed
{
    public static class DbInitializer
    {
        public static async Task Initialize(WardsContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await Seed(context, HorarioBrasilia());
        }

        public static async Task Seed(WardsContext context, DateTime dataAgora)
        {
            #region seed_usuarios           
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddAsync(new Role() { RoleId = (int)UsuarioRoleEnum.Adm, Tipo = nameof(UsuarioRoleEnum.Adm), Descricao = "Administrador do sistema", IsAtivo = true, Data = dataAgora });
                await context.Roles.AddAsync(new Role() { RoleId = (int)UsuarioRoleEnum.Comum, Tipo = GetDescricaoEnum(UsuarioRoleEnum.Comum), Descricao = "Usuário comum", IsAtivo = true, Data = dataAgora });
            }

            if (!await context.Usuarios.AnyAsync())
            {
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 1, NomeCompleto = "Administrador", NomeUsuarioSistema = "adm", Email = "adm@Hotmail.com", Senha = Criptografar("123"), Data = dataAgora, IsAtivo = true });
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 2, NomeCompleto = "Junior Souza", NomeUsuarioSistema = "junioranheu", Email = "junioranheu@Hotmail.com", Senha = Criptografar("123"), Data = dataAgora, IsAtivo = true });
            }

            if (!await context.UsuariosRoles.AnyAsync())
            {
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = (int)UsuarioRoleEnum.Adm });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = (int)UsuarioRoleEnum.Comum });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 2, RoleId = (int)UsuarioRoleEnum.Comum });
            }
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
