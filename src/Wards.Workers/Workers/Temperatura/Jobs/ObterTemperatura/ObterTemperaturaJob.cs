using Microsoft.AspNetCore.Http;
using Quartz;
using System.Text.Json;
using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Domain.Entities;
using Wards.WorkersServices.Workers.Temperatura.Models;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura
{
    [DisallowConcurrentExecution]
    public sealed class ObterTemperaturaJob : IJob, IObterTemperaturaJob
    {
        private readonly ICriarLogCommand _criarLogCommand;

        public ObterTemperaturaJob(ICriarLogCommand criarLogCommand)
        {
            _criarLogCommand = criarLogCommand;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.Clear();

                string latitude = "-22.7272"; // Lorena, SP;
                string longitude = "-45.1199";
                ApiOpenMeteo? resp = ObterTemperatura(latitude, longitude);

                string msg = $"Latitude {latitude} e longitude {longitude}, às {HorarioBrasilia()}, está {resp!.Current_Weather!.Temperature ?? 0}º";
                await Console.Out.WriteLineAsync(msg);

                Log log = new() { Descricao = $"Sucesso no Worker {typeof(ObterTemperaturaJob)} — {msg}", StatusResposta = StatusCodes.Status200OK };
                await _criarLogCommand.Execute(log);
            }
            catch (Exception ex)
            {
                Log log = new() { Descricao = $"Houve um erro no Worker {typeof(ObterTemperaturaJob)}: {ex.Message}", StatusResposta = StatusCodes.Status500InternalServerError };
                await _criarLogCommand.Execute(log);
            }
        }

        private static ApiOpenMeteo? ObterTemperatura(string latitude, string longitude)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true&hourly=temperature_2m,relativehumidity_2m,windspeed_10m";

            HttpClient client = new();
            string resp = client.GetStringAsync(url).Result;
            ApiOpenMeteo? respDeserialize = JsonSerializer.Deserialize<ApiOpenMeteo>(resp.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return respDeserialize;
        }
    }
}