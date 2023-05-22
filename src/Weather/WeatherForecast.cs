using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Weather;

/// <summary>
/// 天气预报信息
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// 是否有效
    /// </summary>
    [JsonIgnore]
    public bool IsValid => !string.IsNullOrEmpty(Status.UpdateTime);

    /// <summary>
    /// 获取状态描述
    /// </summary>
    [JsonProperty(propertyName: "meta")]
    public Meta Status { get; set; }

    /// <summary>
    /// 7天天气预报
    /// </summary>
    [JsonProperty(propertyName: "forecast")]
    public List<ForecastInfo> Forecast { get; set; }

    /// <summary>
    /// 15天天气预报
    /// </summary>
    [JsonProperty(propertyName: "forecast15")]
    public List<ForecastInfo> ForecastFifteenDays { get; set; }

    /// <summary>
    /// 40天天气预报
    /// </summary>
    [JsonProperty(propertyName: "forecast40")]
    public List<ForecastInfo> ForecastFortyDays { get; set; }

    /// <summary>
    /// 当前信息
    /// </summary>
    [JsonProperty(propertyName: "observe")]
    public RealTimeInfo RealTimeWeather { get; set; }

    /// <summary>
    /// 获取当前天气
    /// </summary>
    /// <returns></returns>
    public (WeatherInfo Weather, bool IsNight) GetCurrentWeather()
    {
        if (DateTime.Now.TimeOfDay < new TimeSpan(8, 0, 0))
        {
            var forecastYesterday =
                Forecast
                    .FirstOrDefault(
                        f =>
                            f.DateTime.Date == DateTime.Today.AddDays(-1));

            return (forecastYesterday?.NightWeather, true);
        }

        var forecastToday =
            Forecast
                .FirstOrDefault(
                    f =>
                        f.DateTime.Date == DateTime.Today);

        if (forecastToday == null)
            return (null, false);

        if (DateTime.Now.TimeOfDay >= new TimeSpan(20, 0, 0))
            return (forecastToday.NightWeather, true);

        return (forecastToday.DayWeather, false);
    }
}