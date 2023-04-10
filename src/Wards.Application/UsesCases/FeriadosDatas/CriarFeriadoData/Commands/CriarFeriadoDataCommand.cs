using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using System.Globalization;

namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands
{
    public class CriarFeriadoDataCommand : ICriarFeriadoDataCommand
    {
        private readonly WardsContext _context;

        public CriarFeriadoDataCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(string[] data, int feriadoId)
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

        private static string AdicionarAno0001SeDataInvalida(string data)
        {
            if (DateTime.TryParseExact(data, "dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime _))
                return data;

            return $"{data}/0001";
        }
    }
}