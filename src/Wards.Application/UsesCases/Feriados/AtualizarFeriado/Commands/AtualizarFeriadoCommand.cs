using Wards.Application.UseCases.Feriados.ObterFeriado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Feriados.AtualizarFeriado.Commands
{
    public class AtualizarFeriadoCommand : IAtualizarFeriadoCommand
    {
        private readonly WardsContext _context;
        private readonly IObterFeriadoQuery _obterFeriadoQuery;

        public AtualizarFeriadoCommand(WardsContext context, IObterFeriadoQuery obterFeriadoQuery)
        {
            _context = context;
            _obterFeriadoQuery = obterFeriadoQuery;
        }

        public async Task<int> Execute(Feriado input)
        {
            var item = await _obterFeriadoQuery.Execute(input.FeriadoId);

            if (item is null)
                return 0;

            item.Tipo = input.Tipo != null ? input.Tipo : item.Tipo;
            item.Nome = !string.IsNullOrEmpty(input.Nome) ? input.Nome : item.Nome;
            item.IsMovel = input.IsMovel;
            item.Status = input.Status;
            item.DataAtualizacao = HorarioBrasilia();
            item.UsuarioIdMod = input.UsuarioIdMod;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return item.FeriadoId;
        }
    }
}