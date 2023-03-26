using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Infrastructure.Seed.Seeds
{
    public sealed class SeedEstados
    {
        public static async Task Seed(WardsContext context)
        {
            #region seed_estados
            if (!await context.Estados.AnyAsync())
            {
                await context.Estados.AddAsync(new Estado() { EstadoId = 1, Nome = "Acre", Sigla = "AC", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 2, Nome = "Alagoas", Sigla = "AL", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 3, Nome = "Amapá", Sigla = "AP", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 4, Nome = "Amazonas", Sigla = "AM", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 5, Nome = "Bahia", Sigla = "BA", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 6, Nome = "Ceará", Sigla = "CE", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 7, Nome = "Espírito Santo", Sigla = "ES", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 8, Nome = "Goiás", Sigla = "GO", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 9, Nome = "Maranhão", Sigla = "MA", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 10, Nome = "Mato Grosso", Sigla = "MT", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 11, Nome = "Mato Grosso do Sul", Sigla = "MS", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 12, Nome = "Minas Gerais", Sigla = "MG", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 13, Nome = "Pará", Sigla = "PA", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 14, Nome = "Paraíba", Sigla = "PB", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 15, Nome = "Paraná", Sigla = "PR", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 16, Nome = "Pernambuco", Sigla = "PE", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 17, Nome = "Piauí", Sigla = "PI", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 18, Nome = "Rio de Janeiro", Sigla = "RJ", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 19, Nome = "Rio Grande do Norte", Sigla = "RN", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 20, Nome = "Rio Grande do Sul", Sigla = "RS", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 21, Nome = "Rondônia", Sigla = "RO", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 22, Nome = "Roraima", Sigla = "RR", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 23, Nome = "Santa Catarina", Sigla = "SC", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 24, Nome = "São Paulo", Sigla = "SP", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 25, Nome = "Sergipe", Sigla = "SE", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 26, Nome = "Tocantins", Sigla = "TO", IsAtivo = true });
                await context.Estados.AddAsync(new Estado() { EstadoId = 27, Nome = "Distrito Federal", Sigla = "DF", IsAtivo = true });
            }
            #endregion
        }
    }
}