using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;
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

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Reactive]
        public DateTime LastUpdateTime { get; set; }

        private IDisposable Disposable { get; set; }

        /// <summary>
        /// 获取所有城市
        /// </summary>
        /// <returns></returns>
        public CityKeyInfo[] GetCities()
        {
            return WeatherHelper.Instance.CityKeyInfos.ToArray();
        }

        public void StartUpdate()
        {
            Disposable =
                Observable
                    .Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(100))
                    .Select(_ => DateTime.Now)
                    .Where(NeedUpdate)
                    .Do(_ => UpdateWeather())
                    .Subscribe();
        }

        public void StopUpdate()
        {
            Disposable.Dispose();
        }

        public WeatherForecast UpdateWeather()
        {
            Forecast = City == null ? null : UpdateWeather(City);
            return Forecast;
        }

        public WeatherForecast UpdateWeather(CityKeyInfo city)
        {
            LastUpdateTime = DateTime.Now;
            Forecast = WeatherHelper.Instance.UpdateWeather(city);
            City = city;
            return Forecast;
        }

        private bool NeedUpdate(DateTime time)
        {
            if (time.Year != LastUpdateTime.Year ||
                time.Month != LastUpdateTime.Month ||
                time.Day != LastUpdateTime.Day ||
                time.Hour != LastUpdateTime.Hour)
                return true;

            if (time.Minute != 0 && time.Minute != 30)
                return false;

            if (time.Year == LastUpdateTime.Year &&
                time.Month == LastUpdateTime.Month &&
                time.Day == LastUpdateTime.Day &&
                time.Hour == LastUpdateTime.Hour &&
                time.Minute == LastUpdateTime.Minute)
                return false;

            return true;
        }
    }
}
