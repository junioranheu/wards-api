using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;

namespace Wards.Infrastructure.Seed
{
    public static class DbInitializer
    {
        public static async Task Initialize(WardsContext context)
        {
            // Exclui o esquema, copia as queries, cria esquema/tabelas, popula o BD;
            if (false)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                await Seed(context, HorarioBrasilia());
            }
        }

        public static async Task Seed(WardsContext context, DateTime dataAgora)
        {
            #region seed_usuarios           
            if (!await context.UsuariosRoles.AnyAsync())
            {
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioRoleId = (int)UsuarioRoleEnum.Administrador, Tipo = nameof(UsuarioRoleEnum.Administrador), Descricao = "Administrador do sistema", IsAtivo = true, DataRegistro = dataAgora });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioRoleId = (int)UsuarioRoleEnum.Comum, Tipo = GetDescricaoEnum(UsuarioRoleEnum.Comum), Descricao = "Usuário comum", IsAtivo = true, DataRegistro = dataAgora });
            }

            if (!await context.Usuarios.AnyAsync())
            {
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 1, NomeCompleto = "Administrador do GeekSpot", Email = "adm@Hotmail.com", Senha = Criptografar("123"), Data = dataAgora, UsuarioRoleId = (int)UsuarioRoleEnum.Administrador, IsAtivo = true });
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 2, NomeCompleto = "Junior Souza", Email = "juninholorena@Hotmail.com", Senha = Criptografar("123"), Data = dataAgora, UsuarioRoleId = (int)UsuarioRoleEnum.Comum, IsAtivo = true });
            }
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
