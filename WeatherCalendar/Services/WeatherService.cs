using Weather;

namespace WeatherCalendar.Services
{
    public class WeatherService
    {
        public CityKeyInfo[] GetCities()
        {
            return WeatherHelper.Instance.CityKeyInfos.ToArray();
        }

        public WeatherForecast GetWeather(CityKeyInfo city)
        {
            return WeatherHelper.Instance.UpdateWeather(city);
        }
    }
}
