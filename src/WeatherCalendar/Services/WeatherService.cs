using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Weather;

namespace WeatherCalendar.Services;

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
        if (Disposable != null)
            return;

        var appService = Locator.Current.GetService<AppService>();

        Disposable =
            appService
                .TimerPerSecond
                .Select(_ => DateTime.Now)
                .Where(NeedUpdate)
                .Do(_ => UpdateWeather())
                .Retry()
                .Subscribe();
    }

    public void StopUpdate()
    {
        Disposable?.Dispose();
        Disposable = null;
    }

    public WeatherForecast UpdateWeather()
    {
        return UpdateWeather(City);
    }

    public async Task<WeatherForecast> UpdateWeatherAsync()
    {
        return await Task.Run(() => UpdateWeather(City));
    }

    public WeatherForecast UpdateWeather(CityKeyInfo city)
    {
        City = city;

        WeatherForecast result = null;
        if (city != null)
        {
            LastUpdateTime = DateTime.Now;
            result = WeatherHelper.Instance.UpdateWeather(city);
        }

        if (result != null)
            Forecast = result;

        return result;
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
