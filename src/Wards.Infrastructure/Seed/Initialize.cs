using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed.Seeds;
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
                await context.Roles.AddAsync(new Role() { RoleId = UsuarioRoleEnum.Adm, Tipo = nameof(UsuarioRoleEnum.Adm), Descricao = "Administrador do sistema", IsAtivo = true, Data = dataAgora });
                await context.Roles.AddAsync(new Role() { RoleId = UsuarioRoleEnum.Comum, Tipo = GetDescricaoEnum(UsuarioRoleEnum.Comum), Descricao = "Usuário comum", IsAtivo = true, Data = dataAgora });
            }

            if (!await context.Usuarios.AnyAsync())
            {
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 1, NomeCompleto = "Administrador", NomeUsuarioSistema = "adm", Email = "adm@Hotmail.com", Senha = Criptografar("123"), Chamado = "Chamado #1", HistPerfisAtivos = "1, 2", Data = dataAgora, IsAtivo = true });
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 2, NomeCompleto = "Junior Souza", NomeUsuarioSistema = "junioranheu", Email = "junioranheu@gmail.com", Chamado = "Chamado #2", Senha = Criptografar("123"), HistPerfisAtivos = "1", Data = dataAgora, IsAtivo = true });
            }

            if (!await context.UsuariosRoles.AnyAsync())
            {
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = UsuarioRoleEnum.Adm });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = UsuarioRoleEnum.Comum });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 2, RoleId = UsuarioRoleEnum.Comum });
            }
            #endregion

            #region seed_outros
            await SeedWards.Seed(context);
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
