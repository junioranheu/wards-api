namespace Wards.WorkersServices.Workers.Temperatura.Models
{
    public sealed class ApiOpenMeteo
    {
        public double? Latitude { get; set; } = 0;

        public double? Longitude { get; set; } = 0;

        public CurrentWeather? Current_Weather { get; set; }
    }

    public sealed class CurrentWeather
    {
        public double? Temperature { get; set; } = 0;

        public double? WindSpeed { get; set; } = 0;

        public double? WindDirection { get; set; } = 0;

        public double? WeatherCode { get; set; } = 0;

        public int? Is_Day { get; set; } = 1;

        public DateTime? Time { get; set; } = DateTime.MinValue;
    }
}