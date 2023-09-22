using GeekSpot.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed.Seeds;
using static Wards.Utils.Fixtures.Encrypt;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Infrastructure.Seed
{
    public static class DbInitializer
    {
        public static async Task Initialize(WardsContext context, bool isAplicarMigrations, bool isResetar)
        {
            context.Database.SetCommandTimeout(600);
            // string script = context.Database.GenerateCreateScript();

            if (isAplicarMigrations)
            {
                await context.Database.MigrateAsync();
            }
            else if (isResetar)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            if (isAplicarMigrations || isResetar)
            {
                await Seed(context, GerarHorarioBrasilia());
            }
        }

        public static async Task Seed(WardsContext context, DateTime dataAgora)
        {
            #region seed_usuarios           
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddAsync(new Role() { RoleId = UsuarioRoleEnum.Administrador, Tipo = nameof(UsuarioRoleEnum.Administrador), Descricao = "Administrador do sistema", IsAtivo = true, Data = dataAgora });
                await context.Roles.AddAsync(new Role() { RoleId = UsuarioRoleEnum.Comum, Tipo = ObterDescricaoEnum(UsuarioRoleEnum.Comum), Descricao = "Usuário comum", IsAtivo = true, Data = dataAgora });
            }

            if (!await context.Usuarios.AnyAsync())
            {
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 1, NomeCompleto = "Administrador", NomeUsuarioSistema = "adm", Email = "adm@Hotmail.com", Senha = Criptografar("123"), Chamado = "Chamado #1", Foto = "1AAAAA.jpg", CodigoVerificacao = GerarStringAleatoria(6, true), ValidadeCodigoVerificacao = GerarHorarioBrasilia().AddHours(24), HistPerfisAtivos = "1, 2" });
                await context.Usuarios.AddAsync(new Usuario() { UsuarioId = 2, NomeCompleto = "Junior Souza", NomeUsuarioSistema = "junioranheu", Email = "junioranheu@gmail.com", Senha = Criptografar("123"), Chamado = "Chamado #2", Foto = "2AAAAA.jpg", CodigoVerificacao = GerarStringAleatoria(6, true), ValidadeCodigoVerificacao = GerarHorarioBrasilia().AddHours(24), HistPerfisAtivos = "1" });
            }

            if (!await context.UsuariosRoles.AnyAsync())
            {
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = UsuarioRoleEnum.Administrador });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 1, RoleId = UsuarioRoleEnum.Comum });
                await context.UsuariosRoles.AddAsync(new UsuarioRole() { UsuarioId = 2, RoleId = UsuarioRoleEnum.Comum });
            }
            #endregion

            #region seed_outros
            await SeedWards.Seed(context);
            await SeedEstados.Seed(context);
            await SeedCidades.Seed(context);

            if (!await context.Hashtags.AnyAsync())
            {
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 1, Tag = "#csharp" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 2, Tag = "#dotnet" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 3, Tag = "#reactjs" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 4, Tag = "#frontend" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 5, Tag = "#backend" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 6, Tag = "#mysql" });
                await context.Hashtags.AddAsync(new Hashtag() { HashtagId = 7, Tag = "#visualstudio" });
            }

            if (!await context.WardsHashtags.AnyAsync())
            {
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 1, WardId = 1, HashtagId = 1 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 2, WardId = 1, HashtagId = 2 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 3, WardId = 1, HashtagId = 5 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 4, WardId = 2, HashtagId = 1 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 5, WardId = 2, HashtagId = 2 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 6, WardId = 2, HashtagId = 5 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 7, WardId = 3, HashtagId = 7 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 8, WardId = 4, HashtagId = 7 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 9, WardId = 5, HashtagId = 1 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 10, WardId = 5, HashtagId = 2 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 11, WardId = 5, HashtagId = 5 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 12, WardId = 6, HashtagId = 1 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 13, WardId = 6, HashtagId = 2 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 14, WardId = 6, HashtagId = 5 });

                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 15, WardId = 7, HashtagId = 3 });
                await context.WardsHashtags.AddAsync(new WardHashtag() { WardHashtagId = 16, WardId = 7, HashtagId = 4 });
            }

            // PS: Feriados, FeriadosDatas & FeriadosEstados deve-se aplicar a Stored Procedure: /Infrastructure/StoredProcedures/[Banco]/[Banco][SP_FERIADOS_TEMP][Full].txt;
            //if (!await context.Feriados.AnyAsync())
            //{
            //    await context.Feriados.AddAsync(new Feriado() { FeriadoId = 1, Tipo = TipoFeriadoEnum.Nacional, Nome = "Natal", IsMovel = false, UsuarioId = 1 });
            //    await context.Feriados.AddAsync(new Feriado() { FeriadoId = 2, Tipo = TipoFeriadoEnum.Nacional, Nome = "Carnaval", IsMovel = true, UsuarioId = 1 });
            //    await context.Feriados.AddAsync(new Feriado() { FeriadoId = 3, Tipo = TipoFeriadoEnum.Estadual, Nome = "Feriado do Junior", IsMovel = false, UsuarioId = 1 });
            //}

            //if (!await context.FeriadosDatas.AnyAsync())
            //{
            //    await context.FeriadosDatas.AddAsync(new FeriadoData() { FeriadoDataId = 1, Data = new DateTime(0001, 12, 25), FeriadoId = 1 });
            //    await context.FeriadosDatas.AddAsync(new FeriadoData() { FeriadoDataId = 2, Data = new DateTime(2024, 03, 22), FeriadoId = 2 });
            //    await context.FeriadosDatas.AddAsync(new FeriadoData() { FeriadoDataId = 3, Data = new DateTime(2023, 02, 21), FeriadoId = 2 });
            //    await context.FeriadosDatas.AddAsync(new FeriadoData() { FeriadoDataId = 4, Data = new DateTime(0001, 03, 25), FeriadoId = 3 });
            //}

            //if (!await context.FeriadosEstados.AnyAsync())
            //{
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 1, EstadoId = 24, FeriadoId = 1 });
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 2, EstadoId = 18, FeriadoId = 1 });

            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 3, EstadoId = 24, FeriadoId = 2 });
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 4, EstadoId = 18, FeriadoId = 2 });
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 5, EstadoId = 24, FeriadoId = 2 });
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 6, EstadoId = 18, FeriadoId = 2 });
            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 7, EstadoId = 18, FeriadoId = 2 });

            //    await context.FeriadosEstados.AddAsync(new FeriadoEstado() { FeriadoEstadoId = 8, EstadoId = 18, FeriadoId = 3 });
            //}
            #endregion

            await context.SaveChangesAsync();
        }
    }
}