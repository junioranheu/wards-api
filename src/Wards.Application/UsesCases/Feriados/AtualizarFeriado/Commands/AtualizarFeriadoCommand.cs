using Microsoft.Extensions.Logging;
using Wards.Application.UseCases.Feriados.ObterFeriado.Queries;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Feriados.AtualizarFeriado.Commands
{
    public class AtualizarFeriadoCommand : IAtualizarFeriadoCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;
        private readonly IObterFeriadoQuery _obterFeriadoQuery;

        public AtualizarFeriadoCommand(
            WardsContext context,
            ILogger<ListarEstadoQuery> logger,
            IObterFeriadoQuery obterFeriadoQuery)
        {
            _context = context;
            _logger = logger;
            _obterFeriadoQuery = obterFeriadoQuery;
        }

        public async Task<int> Execute(Feriado input)
        {
            try
            {
                var item = await _obterFeriadoQuery.Execute(input.FeriadoId);

                if (item is null)
                    return 0;

                item.Tipo = input.Tipo != null ? input.Tipo : item.Tipo;
                item.Nome = !string.IsNullOrEmpty(input.Nome) ? input.Nome : item.Nome;
                item.IsMovel = input.IsMovel;
                item.IsAtivo = input.IsAtivo;
                item.DataAtualizacao = HorarioBrasilia();
                item.UsuarioIdMod = input.UsuarioIdMod;

                _context.Update(item);
                await _context.SaveChangesAsync();

                return item.FeriadoId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}