using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Uprise___Solar_Power_Plant.Services;

public class WeatherData
{
    public async Task<Dictionary<DateTime, double>> GetWeatherData()
    {
        string url = "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m";

        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            // Extract timestamps and temperature values
            var timestampStrings = json["hourly"]["time"].ToObject<List<string>>();
            var temperatures = json["hourly"]["temperature_2m"].ToObject<List<double>>();

            // Dictionary to store parsed DateTime and corresponding temperature
            var weatherData = new Dictionary<DateTime, double>();

            for (int i = 0; i < timestampStrings.Count; i++)
            {
                DateTime timestamp = DateTime.ParseExact(timestampStrings[i], "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                weatherData[timestamp] = temperatures[i];
            }

            return weatherData;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
