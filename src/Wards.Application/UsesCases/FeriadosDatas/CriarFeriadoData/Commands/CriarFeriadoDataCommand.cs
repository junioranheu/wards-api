using Microsoft.Extensions.Logging;
using System.Globalization;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands
{
    public class CriarFeriadoDataCommand : ICriarFeriadoDataCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarFeriadoDataCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(string[] data, int feriadoId)
        {
            try
            {
                List<FeriadoData> listFd = new();

                for (int i = 0; i < data?.Length; i++)
                {
                    string dataLoop = AdicionarAno0001SeDataInvalida(data[i]);

                    FeriadoData fd = new()
                    {
                        FeriadoId = feriadoId,
                        Data = DateTime.Parse(dataLoop)
                    };

                    listFd.Add(fd);
                }

                await _context.AddRangeAsync(listFd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }

        private static string AdicionarAno0001SeDataInvalida(string data)
        {
            if (DateTime.TryParseExact(data, "dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime _))
                return data;

            return $"{data}/0001";
        }
    }
}