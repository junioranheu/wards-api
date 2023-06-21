using Wards.Application.UseCases.Feriados.ObterFeriado.Queries;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

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
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            item.Tipo = input.Tipo is not null ? input.Tipo : item.Tipo;
            item.Nome = !string.IsNullOrEmpty(input.Nome) ? input.Nome : item.Nome;
            item.IsMovel = input.IsMovel;
            item.IsAtivo = input.IsAtivo;
            item.DataAtualizacao = GerarHorarioBrasilia();
            item.UsuarioIdMod = input.UsuarioIdMod;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return item.FeriadoId;
        }
    }
}