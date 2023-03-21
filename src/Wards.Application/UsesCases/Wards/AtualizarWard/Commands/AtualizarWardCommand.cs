using Wards.Application.UsesCases.Wards.ObterWard.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.AtualizarWard.Commands
{
    public sealed class AtualizarWardCommand : IAtualizarWardCommand
    {
        private readonly WardsContext _context;
        private readonly IObterWardQuery _obterQuery;

        public AtualizarWardCommand(WardsContext context, IObterWardQuery obterQuery)
        {
            _context = context;
            _obterQuery = obterQuery;
        }

        public async Task<int> Execute(Ward input)
        {
            var item = await _obterQuery.Execute(input.WardId);

            if (item is null)
                return 0;

            item.Conteudo = !String.IsNullOrEmpty(input.Conteudo) ? input.Conteudo : item.Conteudo;
            item.UsuarioModId = input.UsuarioModId;
            item.DataMod = HorarioBrasilia();
            item.IsAtivo = input.IsAtivo;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return input.WardId;
        }
    }
}