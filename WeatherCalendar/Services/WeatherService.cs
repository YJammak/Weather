using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Weather;

namespace WeatherCalendar.Services
{
    public class WeatherService : ReactiveObject
    {
        /// <summary>
        /// 天气预报
        /// </summary>
        [Reactive]
        public WeatherForecast Forecast { get; set; }
        
        /// <summary>
        /// 城市
        /// </summary>
        [Reactive]
        public CityKeyInfo City { get; set; }

        public CityKeyInfo[] GetCities()
        {
            return WeatherHelper.Instance.CityKeyInfos.ToArray();
        }

        public WeatherForecast UpdateWeather()
        {
            Forecast = City == null ? null : UpdateWeather(City);
            return Forecast;
        }

        public WeatherForecast UpdateWeather(CityKeyInfo city)
        {
            Forecast = WeatherHelper.Instance.UpdateWeather(city);
            City = city;
            return Forecast;
        }
    }
}
